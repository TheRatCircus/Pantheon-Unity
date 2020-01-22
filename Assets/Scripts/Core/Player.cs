// Player.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Components.Entity;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cursor = Pantheon.UI.Cursor;

namespace Pantheon.Core
{
    public enum InputMode
    {
        None,
        Cancelling,
        Default,
        Point,
        Line,
        Path,
        Menu
    }

    public sealed class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] private GameObject targetOverlay = default;

        [SerializeField] private Cursor cursor = default;
        [SerializeField] private HUD hud = default;
        private List<GameObject> targetOverlays = new List<GameObject>(10);

        public InputMode Mode { get; set; } = InputMode.Default;

        private Entity entity;
        public Entity Entity
        {
            get => entity;
            set
            {
                entity = value;
                actor = value.GetComponent<Actor>();
                inventory = value.GetComponent<Inventory>();
            }
        }
        private Actor actor;
        private Inventory inventory;

        private int targetingRange;
        private Cell selectedCell;
        private List<Cell> selectedLine = new List<Cell>();
        public HashSet<Entity> VisibleActors { get; private set; }
            = new HashSet<Entity>();
        public List<Cell> AutoMovePath { get; set; }
            = new List<Cell>();

        private void Update()
        {
            if (Mode == InputMode.None)
                return;

            if (!Input.anyKeyDown &&
                Input.GetAxis("MouseX") == 0 &&
                Input.GetAxis("MouseY") == 0)
                return;

            if (cursor.HoveredCell.Visible)
            {
                selectedCell = cursor.HoveredCell;
                selectedLine = Bresenhams.GetLine(Entity.Level, Entity.Cell, selectedCell);
            }

            switch (Mode)
            {
                case InputMode.Default:
                case InputMode.Menu:
                    break;
                case InputMode.Point:
                    PointSelect();
                    return;
                case InputMode.Line:
                    LineSelect();
                    return;
                default:
                    throw new System.NotImplementedException();
            }

            // Set automove path
            if (Input.GetMouseButtonDown(0))
            {
                //AutoMovePath = PlayerEntity.Level.GetPathTo(
                //    PlayerEntity.Cell, cursor.HoveredCell);
                //return;
            }

            if (Input.GetButtonDown("Up"))
            {
                actor.Command = new MoveCommand(Entity, Vector2Int.up);
            }
            else if (Input.GetButtonDown("Down"))
            {
                actor.Command = new MoveCommand(Entity, Vector2Int.down);
            }
            else if (Input.GetButtonDown("Left"))
            {
                actor.Command = new MoveCommand(Entity, Vector2Int.left);
            }
            else if (Input.GetButtonDown("Right"))
            {
                actor.Command = new MoveCommand(Entity, Vector2Int.right);
            }
            else if (Input.GetButtonDown("Up Left"))
            {
                actor.Command = new MoveCommand(Entity, new Vector2Int(-1, 1));
            }
            else if (Input.GetButtonDown("Up Right"))
            {
                actor.Command = new MoveCommand(Entity, new Vector2Int(1, 1));
            }
            else if (Input.GetButtonDown("Down Left"))
            {
                actor.Command = new MoveCommand(Entity, new Vector2Int(-1, -1));
            }
            else if (Input.GetButtonDown("Down Right"))
            {
                actor.Command = new MoveCommand(Entity, new Vector2Int(1, -1));
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Talent talent = Talent.GetAllTalents(Entity)[0];
                Cell target = GetTalentTarget(talent.Targeting);
                actor.Command = new TalentCommand(Entity, talent, target);
            }
            else if (Input.GetButtonDown("Wait"))
                actor.Command = new WaitCommand(Entity);
            else if (Input.GetButtonDown("Use"))
            {
                actor.Command = new UseItemCommand(Entity,
                    Entity.GetComponent<Inventory>().Items[0]);
            }
            else if (Input.GetButtonDown("Pickup"))
                actor.Command = new PickupCommand(Entity);
            else if (Input.GetButtonDown("Inventory"))
            {
                if (Mode == InputMode.Menu) // Clear modal if already in one
                {
                    hud.ClearModalList();
                    Mode = InputMode.Default;
                    return;
                }

                Mode = InputMode.Menu;
                Inventory inv = Entity.GetComponent<Inventory>();
                ModalList ml = hud.OpenModalList();
                ml.SetPrompt("Inventory");
                ml.Populate(inv.Items.Count);
                for (int i = 0; i < inv.Items.Count; i++)
                {
                    ml.SetOptionImage(i, inv.Items[i].Flyweight.Sprite);
                    ml.SetOptionText(i, inv.Items[i].Name);
                    ml.SetOptionCallback(i, delegate (int optionNo)
                    {
                        Entity e = inv.Items[optionNo];
                        actor.Command = new UseItemCommand(Entity, e);
                        Mode = InputMode.Default;
                    });
                }
            }
            else if (Input.GetButtonDown("Toss"))
                actor.Command = new TossCommand(Entity,
                    Entity.GetComponent<Inventory>().Items[0]);
            else if (Input.GetButtonDown("Wield"))
                actor.Command = new WieldCommand(Entity,
                    Entity.GetComponent<Inventory>().Items[0]);
        }

        private void PointSelect()
        {
            bool withinRange = Entity.Level.Distance(
                Entity.Cell, cursor.HoveredCell) < targetingRange;

            CleanOverlays();

            if (withinRange)
            {
                GameObject overlayObj = Instantiate(
                   targetOverlay,
                   cursor.HoveredCell.Position.ToVector3(),
                   new Quaternion());

                targetOverlays.Add(overlayObj);
            }

            if (Input.GetMouseButtonDown(0) && withinRange)
            {
                selectedCell = cursor.HoveredCell;
                Mode = InputMode.Default;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                selectedCell = null;
                Mode = InputMode.Cancelling;
                Locator.Log.Send("Targeting cancelled.", Color.blue);
            }
        }

        private void LineSelect()
        {
            bool withinRange = Entity.Level.Distance(
                Entity.Cell, cursor.HoveredCell) < targetingRange;

            CleanOverlays();

            List<Cell> line = new List<Cell>();

            if (withinRange)
            {
                line = Bresenhams.GetLine(
                    Entity.Level,
                    Entity.Cell,
                    cursor.HoveredCell);
                foreach (Cell c in line)
                {
                   GameObject overlayObj = Instantiate(
                       targetOverlay,
                       c.Position.ToVector3(),
                       new Quaternion());

                    targetOverlays.Add(overlayObj);
                }
            }

            if (Input.GetMouseButtonDown(0) && withinRange)
            {
                selectedLine = line;
                Mode = InputMode.Default;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                selectedLine.Clear();
                Mode = InputMode.Cancelling;
                Locator.Log.Send("Targeting cancelled.", Color.blue);
            }
        }

        public void RecalculateVisible(IEnumerable<Cell> cells)
        {
            VisibleActors.Clear();
            foreach (Cell c in cells)
            {
                if (c.Actor != null
                    && c.Actor.GetComponent<Actor>().Control != ActorControl.Player)
                    VisibleActors.Add(c.Actor);
            }
        }

        public InputMode RequestCell(out Cell cell, int range)
        {
            switch (Mode)
            {
                case InputMode.Default: // Start polling for cell
                    targetingRange = range;
                    Mode = InputMode.Point;
                    PointSelect();
                    cell = null;
                    return Mode;
                case InputMode.Cancelling: // Stop polling for cell
                    Mode = InputMode.Default;
                    cell = null;
                    CleanOverlays();
                    return InputMode.Cancelling;
                case InputMode.Point:
                    if (selectedCell == null)
                        // Still no selection
                        cell = null;
                    else
                    {
                        // Selection made
                        Mode = InputMode.Default;
                        cell = selectedCell;
                        selectedCell = null;
                        CleanOverlays();
                    }
                    return Mode;
                default:
                    throw new System.Exception(
                        "PlayerControl is in an illegal input mode.");
            }
        }

        public InputMode RequestLine(out List<Cell> line, int range)
        {
            switch (Mode)
            {
                case InputMode.Default: // Start polling for line
                    targetingRange = range;
                    Mode = InputMode.Line;
                    LineSelect();
                    line = null;
                    return Mode;
                case InputMode.Cancelling: // Stop polling for line
                    Mode = InputMode.Default;
                    line = null;
                    CleanOverlays();
                    return InputMode.Cancelling;
                case InputMode.Line:
                    if (selectedLine.Count < 1)
                        // Still no selection
                        line = null;
                    else
                    {
                        // Selection made
                        Mode = InputMode.Default;
                        line = new List<Cell>(selectedLine);
                        selectedLine.Clear();
                        CleanOverlays();
                    }
                    return Mode;
                default:
                    throw new System.Exception(
                        "PlayerControl is in an illegal input mode.");
            }
        }

        private void CleanOverlays()
        {
            foreach (GameObject go in targetOverlays)
                Destroy(go);
            targetOverlays.Clear();
        }

        private Cell GetTalentTarget(TalentTargeting targeting)
        {
            switch (targeting)
            {
                case TalentTargeting.Adjacent:
                    return selectedLine.ElementAtOrDefault(1);
                case TalentTargeting.None:
                    return null;
                default:
                    throw new System.ArgumentException(
                        "Invalid talent targeting scheme.");
            }
        }

        private void OnDrawGizmos()
        {
            if (selectedLine == null || selectedCell == null)
                return;

            foreach (Cell cell in selectedLine)
                Gizmos.DrawCube(
                    cell.Position.ToVector3(),
                    new Vector3(.2f, .2f, .2f));

            Gizmos.color = Color.red;
            Gizmos.DrawCube(
                selectedCell.Position.ToVector3(),
                new Vector3(.3f, .3f, .3f));
        }
    }
}

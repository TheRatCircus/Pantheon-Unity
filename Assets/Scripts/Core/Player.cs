// Player.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Components;
using Pantheon.Content.Talents;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
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
        [SerializeField] private Hotbar hotbarUI = default;
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
            }
        }
        private Actor actor;

        private int targetingRange;

        private Vector2Int selectedCell = Level.NullCell;
        private Line selectedLine = new Line();

        public HashSet<Entity> VisibleActors { get; private set; }
            = new HashSet<Entity>();

        public Talent[] Hotbar = new Talent[10];
        public int HotbarSelection;

        public void FillHotbar(Talent[] talents)
        {
            Hotbar = talents;

            for (int i = 0; i < Hotbar.Length; i++)
            {
                if (Hotbar[i] != null)
                    hotbarUI.SetSprite(i, Hotbar[i].Icon);
            }
        }

        private void Update()
        {
            if (Mode == InputMode.None)
                return;

            if (!Input.anyKeyDown &&
                Input.GetAxis("MouseX") == 0 &&
                Input.GetAxis("MouseY") == 0)
                return;

            if (Entity.Level.CellIsVisible(cursor.HoveredCell))
            {
                selectedCell = cursor.HoveredCell;
                selectedLine = Bresenhams.GetLine(Entity.Position, selectedCell);
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

            if (Input.GetButtonDown("Up"))
            {
                actor.Command = MoveCommand.Directional(Entity, Vector2Int.up);
            }
            else if (Input.GetButtonDown("Down"))
            {
                actor.Command = MoveCommand.Directional(Entity, Vector2Int.down);
            }
            else if (Input.GetButtonDown("Left"))
            {
                actor.Command = MoveCommand.Directional(Entity, Vector2Int.left);
            }
            else if (Input.GetButtonDown("Right"))
            {
                actor.Command = MoveCommand.Directional(Entity, Vector2Int.right);
            }
            else if (Input.GetButtonDown("Up Left"))
            {
                actor.Command = MoveCommand.Directional(Entity, new Vector2Int(-1, 1));
            }
            else if (Input.GetButtonDown("Up Right"))
            {
                actor.Command = MoveCommand.Directional(Entity, new Vector2Int(1, 1));
            }
            else if (Input.GetButtonDown("Down Left"))
            {
                actor.Command = MoveCommand.Directional(Entity, new Vector2Int(-1, -1));
            }
            else if (Input.GetButtonDown("Down Right"))
            {
                actor.Command = MoveCommand.Directional(Entity, new Vector2Int(1, -1));
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
            else if (Input.GetMouseButtonDown(0))
            {
                actor.Command = new TalentCommand(Entity, Hotbar[HotbarSelection]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                hotbarUI.SetSelected(0);
                HotbarSelection = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                hotbarUI.SetSelected(1);
                HotbarSelection = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                hotbarUI.SetSelected(2);
                HotbarSelection = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                hotbarUI.SetSelected(3);
                HotbarSelection = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                hotbarUI.SetSelected(4);
                HotbarSelection = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                hotbarUI.SetSelected(5);
                HotbarSelection = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                hotbarUI.SetSelected(6);
                HotbarSelection = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                hotbarUI.SetSelected(7);
                HotbarSelection = 7;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                hotbarUI.SetSelected(8);
                HotbarSelection = 8;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                hotbarUI.SetSelected(9);
                HotbarSelection = 9;
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                FloodFill.FillLevel(Entity.Level, Entity.Position,
                    delegate (Level level, Vector2Int pos) 
                    {
                        return level.CellIsVisible(pos);
                    });
            }
        }

        private void PointSelect()
        {
            bool withinRange = Level.Distance(
                Entity.Position, cursor.HoveredCell) < targetingRange;

            CleanOverlays();

            if (withinRange)
            {
                GameObject overlayObj = Instantiate(
                   targetOverlay,
                   cursor.HoveredCell.ToVector3(),
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
                selectedCell = Level.NullCell;
                Mode = InputMode.Cancelling;
                Locator.Log.Send("Targeting cancelled.", Color.blue);
            }
        }

        private void LineSelect()
        {
            bool withinRange = Level.Distance(
                Entity.Position, cursor.HoveredCell) < targetingRange;

            CleanOverlays();

            Line line = new Line();

            if (withinRange)
            {
                line = Bresenhams.GetLine(Entity.Position, cursor.HoveredCell);
                foreach (Vector2Int c in line)
                {
                   GameObject overlayObj = Instantiate(
                       targetOverlay,
                       c.ToVector3(),
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

        public void RecalculateVisible(IEnumerable<Vector2Int> cells)
        {
            Level level = Entity.Level;
            VisibleActors.Clear();
            foreach (Vector2Int cell in cells)
            {
                Entity actor = level.ActorAt(cell);
                if (actor != null
                    && actor.GetComponent<Actor>().Control != ActorControl.Player)
                {
                    VisibleActors.Add(actor);
                }
            }
        }

        public InputMode RequestCell(out Vector2Int cell, int range)
        {
            switch (Mode)
            {
                case InputMode.Default: // Start polling for cell
                    targetingRange = range;
                    Mode = InputMode.Point;
                    PointSelect();
                    cell = Level.NullCell;
                    return Mode;
                case InputMode.Cancelling: // Stop polling for cell
                    Mode = InputMode.Default;
                    cell = Level.NullCell;
                    CleanOverlays();
                    return InputMode.Cancelling;
                case InputMode.Point:
                    if (selectedCell == null)
                        // Still no selection
                        cell = Level.NullCell;
                    else
                    {
                        // Selection made
                        Mode = InputMode.Default;
                        cell = selectedCell;
                        selectedCell = Level.NullCell;
                        CleanOverlays();
                    }
                    return Mode;
                default:
                    throw new System.Exception(
                        "PlayerControl is in an illegal input mode.");
            }
        }

        public InputMode RequestLine(out Line line, int range)
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
                        line = new Line(selectedLine);
                        selectedLine.Clear();
                        CleanOverlays();
                    }
                    return Mode;
                default:
                    throw new System.Exception(
                        "PlayerControl is in an illegal input mode.");
            }
        }

        public Vector2Int GetTargetedAdjacent()
        {
            if (selectedLine.Count < 1)
                return Level.NullCell;
            else
                return selectedLine[1];
        }

        private void CleanOverlays()
        {
            foreach (GameObject go in targetOverlays)
                Destroy(go);
            targetOverlays.Clear();
        }

        private void OnDrawGizmos()
        {
            foreach (Vector2Int cell in selectedLine)
                Gizmos.DrawCube(cell.ToVector3(), new Vector3(.2f, .2f, .2f));

            Gizmos.color = Color.red;
            Gizmos.DrawCube(selectedCell.ToVector3(), new Vector3(.3f, .3f, .3f));
        }
    }
}

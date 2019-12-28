// PlayerControl.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Components;
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

    public sealed class PlayerControl : MonoBehaviour, IPlayerControl
    {
        [SerializeField] private GameObject targetOverlay = default;

        [SerializeField] private Cursor cursor = default;
        [SerializeField] private HUD hud = default;
        private List<GameObject> targetOverlays = new List<GameObject>(10);

        public InputMode Mode { get; set; } = InputMode.Default;

        private Entity playerEntity;
        public Entity PlayerEntity
        {
            get => playerEntity;
            set
            {
                playerEntity = value;
                playerActor = value.GetComponent<Actor>();
                inventory = value.GetComponent<Inventory>();
            }
        }
        private Actor playerActor;
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
                playerActor.Command = new MoveCommand(PlayerEntity, Vector2Int.up);
            }
            else if (Input.GetButtonDown("Down"))
            {
                playerActor.Command = new MoveCommand(PlayerEntity, Vector2Int.down);
            }
            else if (Input.GetButtonDown("Left"))
            {
                playerActor.Command = new MoveCommand(PlayerEntity, Vector2Int.left);
            }
            else if (Input.GetButtonDown("Right"))
            {
                playerActor.Command = new MoveCommand(PlayerEntity, Vector2Int.right);
            }
            else if (Input.GetButtonDown("Up Left"))
            {
                playerActor.Command = new MoveCommand(PlayerEntity, new Vector2Int(-1, 1));
            }
            else if (Input.GetButtonDown("Up Right"))
            {
                playerActor.Command = new MoveCommand(PlayerEntity, new Vector2Int(1, 1));
            }
            else if (Input.GetButtonDown("Down Left"))
            {
                playerActor.Command = new MoveCommand(PlayerEntity, new Vector2Int(-1, -1));
            }
            else if (Input.GetButtonDown("Down Right"))
            {
                playerActor.Command = new MoveCommand(PlayerEntity, new Vector2Int(1, -1));
            }
            else if (Input.GetButtonDown("Wait"))
                playerActor.Command = new WaitCommand(PlayerEntity);
            else if (Input.GetButtonDown("Use"))
            {
                playerActor.Command = new UseItemCommand(PlayerEntity,
                    PlayerEntity.GetComponent<Inventory>().Items[0]);
            }
            else if (Input.GetButtonDown("Autoattack"))
            {
                playerActor.Command = Autoattack();
            }
            else if (Input.GetButtonDown("Pickup"))
                playerActor.Command = new PickupCommand(PlayerEntity);
            else if (Input.GetButtonDown("Inventory"))
            {
                if (Mode == InputMode.Menu) // Clear modal if already in one
                {
                    hud.ClearModalList();
                    Mode = InputMode.Default;
                    return;
                }

                Mode = InputMode.Menu;
                Inventory inv = PlayerEntity.GetComponent<Inventory>();
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
                        playerActor.Command = new UseItemCommand(PlayerEntity, e);
                        Mode = InputMode.Default;
                    });
                }
            }
            else if (Input.GetButtonDown("Toss"))
                playerActor.Command = new TossCommand(PlayerEntity,
                    PlayerEntity.GetComponent<Inventory>().Items[0]);
        }

        private void PointSelect()
        {
            bool withinRange = PlayerEntity.Level.Distance(
                PlayerEntity.Cell, cursor.HoveredCell) < targetingRange;

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
            bool withinRange = PlayerEntity.Level.Distance(
                PlayerEntity.Cell, cursor.HoveredCell) < targetingRange;

            CleanOverlays();

            List<Cell> line = new List<Cell>();

            if (withinRange)
            {
                line = Bresenhams.GetLine(
                    PlayerEntity.Level,
                    PlayerEntity.Cell,
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

        private ActorCommand Autoattack()
        {
            HashSet<Entity> visibleEnemies = new HashSet<Entity>();

            foreach (Entity npc in VisibleActors)
            {
                if (npc.GetComponent<Actor>().HostileTo(playerActor))
                    visibleEnemies.Add(npc);
            }

            if (visibleEnemies.Count < 1)
            {
                Locator.Log.Send("No visible enemies.", Color.grey);
                return null;
            }

            Entity target = null;
            int distance = 255;

            foreach (Entity enemy in visibleEnemies)
            {
                int d = playerEntity.Level.Distance(enemy.Cell, playerEntity.Cell);
                if (d < distance)
                {
                    distance = d;
                    target = enemy;
                }
            }

            Cell nearestEnemyCell = target.Cell;

            if (!playerEntity.Level.AdjacentTo(playerEntity.Cell, nearestEnemyCell))
            {
                List<Cell> path = playerEntity.Level.GetPathTo(playerEntity.Cell, nearestEnemyCell);
                if (path.Count > 0)
                    return new MoveCommand(playerEntity, path[0]);
                else
                {
                    Locator.Log.Send(
                        $"Cannot find a path to {nearestEnemyCell.Actor.ToSubjectString(false)}.",
                        Color.grey);
                }
            }

            return new MeleeCommand(playerEntity, nearestEnemyCell);
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
    }
}

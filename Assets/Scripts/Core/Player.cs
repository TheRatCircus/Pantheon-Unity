// Player.cs
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
            else if (Input.GetButtonDown("Wait"))
                actor.Command = new WaitCommand(Entity);
            else if (Input.GetButtonDown("Use"))
            {
                actor.Command = new UseItemCommand(Entity,
                    Entity.GetComponent<Inventory>().Items[0]);
            }
            else if (Input.GetButtonDown("Autoattack"))
            {
                actor.Command = Autoattack();
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
            else if (Input.GetButtonDown("Evoke"))
            {
                Wield wield = Entity.GetComponent<Wield>();
                if (!wield.Wielding)
                {
                    Locator.Log.Send("You aren't wielding anything.",
                        Color.grey);
                    return;
                }
                else
                {
                    actor.Command = new EvokeCommand(
                        Entity, wield.Items[0]);
                }
            }
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

        private ActorCommand Autoattack()
        {
            HashSet<Entity> visibleEnemies = new HashSet<Entity>();

            foreach (Entity npc in VisibleActors)
            {
                if (npc.GetComponent<Actor>().HostileTo(actor))
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
                int d = entity.Level.Distance(enemy.Cell, entity.Cell);
                if (d < distance)
                {
                    distance = d;
                    target = enemy;
                }
            }

            Cell cell = target.Cell;
            List<Cell> line = Bresenhams.GetLine(entity.Level, entity.Cell, cell);
            List<Vector2Int> path = entity.Level.GetPathTo(entity.Cell, cell);

            if (entity.TryGetComponent(out Wield wield))
            {
                Entity[] evocables = wield.GetEvocables();
                if (evocables.Length > 0)
                {
                    Talent talent = evocables[0].GetComponent<Evocable>().Talents[0];
                    if (talent.Range >= distance)
                    {
                        EvokeCommand cmd = new EvokeCommand(entity, evocables[0])
                        {
                            Cell = cell,
                            Line = line,
                            Path = path
                        };
                        return cmd;
                    }
                }
            }

            if (!entity.Level.AdjacentTo(entity.Cell, cell))
            {
                if (path.Count > 0)
                    return new MoveCommand(entity, path[0]);
                else
                {
                    Locator.Log.Send(
                        $"Cannot find a path to {cell.Actor.ToSubjectString(false)}.",
                        Color.grey);
                    return null;
                }
            }

            return new MeleeCommand(entity, cell);
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

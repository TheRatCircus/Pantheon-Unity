// Player.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Components.Entity;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
        [SerializeField] private Tile targetOverlayTile = default;

        [SerializeField] private Cursor cursor = default;
        [SerializeField] private HUD hud = default;
        [SerializeField] private Hotbar hotbarUI = default;
        [SerializeField] private Tilemap targetingTilemap = default;

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
        private Vector2Int selectedCell;
        private Line selectedLine = new Line();
        public List<Vector2Int> VisibleCells { get; private set; }
            = new List<Vector2Int>();

        public Talent[] Talents { get; private set; } = new Talent[10];
        private int hotbarSelection;

        private void Start()
        {
            //Entity.GetComponent<Talented>().TalentChangeEvent += UpdateHotbar;
            if (Entity.TryGetComponent(out Wield wield))
                wield.WieldChangeEvent += UpdateHotbar;
            selectedCell = Entity.Cell;
            selectedLine = Bresenhams.GetLine(Entity.Level, Entity.Cell, selectedCell);
            VisibleCells = Floodfill.StackFillIf(
                Entity.Level,
                Entity.Cell,
                (Vector2Int c) => Entity.Level.Visible(c.x, c.y));
        }

        private void Update()
        {
            if (Mode == InputMode.None)
                return;

            if (!Input.anyKeyDown &&
                Input.GetAxis("MouseX") == 0 &&
                Input.GetAxis("MouseY") == 0)
                return;

            if (cursor.HoveredCell != null &&
                Entity.Level.Visible(cursor.HoveredCell.x, cursor.HoveredCell.y))
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
                actor.Command = new MoveCommand(Entity, 0, 1);
            }
            else if (Input.GetButtonDown("Down"))
            {
                actor.Command = new MoveCommand(Entity, 0, -1);
            }
            else if (Input.GetButtonDown("Left"))
            {
                actor.Command = new MoveCommand(Entity, -1, 0);
            }
            else if (Input.GetButtonDown("Right"))
            {
                actor.Command = new MoveCommand(Entity, 1, 0);
            }
            else if (Input.GetButtonDown("Up Left"))
            {
                actor.Command = new MoveCommand(Entity, -1, 1);
            }
            else if (Input.GetButtonDown("Up Right"))
            {
                actor.Command = new MoveCommand(Entity, 1, 1);
            }
            else if (Input.GetButtonDown("Down Left"))
            {
                actor.Command = new MoveCommand(Entity, -1, -1);
            }
            else if (Input.GetButtonDown("Down Right"))
            {
                actor.Command = new MoveCommand(Entity, 1, -1);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Talent talent = Talents[hotbarSelection];
                if (talent != null)
                {
                    Vector2Int target = selectedCell;
                    actor.Command = new TalentCommand(Entity, talent, target);
                }
            }
            else if (Input.GetButtonDown("Wait"))
                actor.Command = new WaitCommand(Entity);
            else if (Input.GetButtonDown("Use"))
            {
                // TODO: GUI
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
                // TODO: GUI
                actor.Command = new TossCommand(Entity,
                    Entity.GetComponent<Inventory>().Items[0]);
            else if (Input.GetButtonDown("Wield"))
                // TODO: GUI
                actor.Command = new WieldCommand(Entity,
                    Entity.GetComponent<Inventory>().Items[0]);
            else if (Input.GetButtonDown("Interact"))
            {
                actor.Command = new InteractCommand(Entity);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                hotbarUI.SetSelected(0);
                hotbarSelection = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                hotbarUI.SetSelected(1);
                hotbarSelection = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                hotbarUI.SetSelected(2);
                hotbarSelection = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                hotbarUI.SetSelected(3);
                hotbarSelection = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                hotbarUI.SetSelected(4);
                hotbarSelection = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                hotbarUI.SetSelected(5);
                hotbarSelection = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                hotbarUI.SetSelected(6);
                hotbarSelection = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                hotbarUI.SetSelected(7);
                hotbarSelection = 7;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                hotbarUI.SetSelected(8);
                hotbarSelection = 8;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                hotbarUI.SetSelected(9);
                hotbarSelection = 9;
            }
        }

        private void LateUpdate()
        {
            DrawTalentOverlays();
        }

        private void PointSelect()
        {
            bool withinRange = Helpers.Distance(
                Entity.Cell, cursor.HoveredCell) < targetingRange;

            targetingTilemap.ClearAllTiles();

            if (withinRange)
            {
                targetingTilemap.SetTile(
                    (Vector3Int)cursor.HoveredCell, targetOverlayTile);
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
            bool withinRange = Helpers.Distance(
                Entity.Cell, cursor.HoveredCell) < targetingRange;

            targetingTilemap.ClearAllTiles();

            Line line = new Line();

            if (withinRange)
            {
                line = Bresenhams.GetLine(
                    Entity.Level,
                    Entity.Cell,
                    cursor.HoveredCell);

                foreach (Vector2Int c in line)
                    targetingTilemap.SetTile((Vector3Int)c, targetOverlayTile);
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

        public void UpdateVisibles(IEnumerable<Vector2Int> cells)
        {
            VisibleCells.Clear();
            VisibleCells.AddRange(cells);
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
                    targetingTilemap.ClearAllTiles();
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
                        targetingTilemap.ClearAllTiles();
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
                    targetingTilemap.ClearAllTiles();
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
                        targetingTilemap.ClearAllTiles();
                    }
                    return Mode;
                default:
                    throw new System.Exception(
                        "PlayerControl is in an illegal input mode.");
            }
        }

        private void DrawTalentOverlays()
        {
            // TODO: Move tilemap clear to after null check
            targetingTilemap.ClearAllTiles();

            Talent talent = Talents[hotbarSelection];

            if (talent == null)
                return;

            // TODO: Replace HashSets with Lists here
            HashSet<Vector2Int> targeted = new HashSet<Vector2Int>();
            targeted.AddMany(talent.GetTargetedCells(Entity, selectedCell));
            HashSet<Vector2Int> overlayed = new HashSet<Vector2Int>();

            foreach (Vector2Int c in targeted)
            {
                if (overlayed.Contains(c))
                    continue;

                targetingTilemap.SetTile((Vector3Int)c, targetOverlayTile);
                overlayed.Add(c);
            }
        }

        private void UpdateHotbar(Talent[] talents)
        {
            int i = 0;

            foreach (Talent talent in talents)
            {
                if (talent == null)
                    continue;

                while (Talents[i++] != null)
                {
                    if (i >= Talents.Length)
                        return;
                }
                Talents[i] = talent;
            }

            for (int j = 0; j < Talents.Length; j++)
            {
                if (Talents[j] != null)
                    hotbarUI.SetSprite(j, Talents[j].Icon);
            }
        }

        private void UpdateHotbar(Entity[] items)
        {
            int i = 0;

            foreach (Entity item in items)
            {
                if (item == null || !item.TryGetComponent(out Evocable evoc))
                    continue;

                foreach (Talent talent in evoc.Talents)
                {
                    if (talent == null)
                        continue;

                    while (Talents[i] != null)
                    {
                        if (++i >= Talents.Length)
                            return;
                    }
                    Talents[i] = talent;
                }
            }

            for (int j = 0; j < Talents.Length; j++)
            {
                if (Talents[j] != null)
                    hotbarUI.SetSprite(j, Talents[j].Icon);
            }
        }

        public void UpdateHotbar()
        {
            Talent[] talents = Talent.GetAllTalents(Entity);
            int i = 0;
            foreach (Talent talent in talents)
            {
                if (talent == null)
                    continue;

                while (Talents[i] != null)
                {
                    if (i >= Talents.Length)
                        return;

                    i++;
                }
                Talents[i] = talent;
            }

            for (int j = 0; j < Talents.Length; j++)
            {
                if (Talents[j] != null)
                    hotbarUI.SetSprite(j, Talents[j].Icon);
            }

            hotbarUI.SetSelected(0);
        }

        private void OnDrawGizmos()
        {
            if (selectedLine == null || selectedCell == null)
                return;

            foreach (Vector2Int cell in selectedLine)
                Gizmos.DrawCube(
                    cell.ToVector3(),
                    new Vector3(.2f, .2f, .2f));

            Gizmos.color = Color.red;
            Gizmos.DrawCube(
                selectedCell.ToVector3(),
                new Vector3(.3f, .3f, .3f));
        }
    }
}

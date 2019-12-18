// PlayerControl.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Core
{
    public enum InputMode
    {
        None,
        Cancelling,
        Default,
        Point,
        Line,
        Path
    }

    public sealed class PlayerControl : MonoBehaviour, IPlayerControl
    {
        [SerializeField] private UI.Cursor cursor = default;

        public InputMode Mode { get; set; } = InputMode.Default;

        private Entity playerEntity;
        public Entity PlayerEntity
        {
            get => playerEntity;
            set
            {
                playerEntity = value;
                playerActor = value.GetComponent<Actor>();
            }
        }
        private Actor playerActor;

        private Cell selectedCell;
        public HashSet<Entity> VisibleActors { get; private set; }
            = new HashSet<Entity>();
        public List<Cell> AutoMovePath { get; set; }
            = new List<Cell>();

        private void Update()
        {
            if (Mode == InputMode.None)
                return;

            if (!Input.anyKeyDown)
                return;

            switch (Mode)
            {
                case InputMode.Default:
                    break;
                case InputMode.Point:
                    PointSelect();
                    return;
            }

            // Set automove path
            if (Input.GetMouseButtonDown(0))
            {
                AutoMovePath = PlayerEntity.Level.GetPathTo(
                    PlayerEntity.Cell, cursor.HoveredCell);
                return;
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
            else if (Input.GetButtonDown("Pickup"))
                playerActor.Command = new PickupCommand(PlayerEntity);
            else if (Input.GetButtonDown("Inventory"))
                playerActor.Command = new DropCommand(PlayerEntity);
        }

        private void PointSelect()
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedCell = cursor.HoveredCell;
                Mode = InputMode.Default;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                selectedCell = null;
                Mode = InputMode.Cancelling;
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

        public bool ActorIsVisible(Actor actor)
        {
            foreach (Entity visibleActor in VisibleActors)
                if (visibleActor.GetComponent<Actor>() == actor)
                    return true;

            return false;
        }

        public InputMode RequestCell(out Cell cell)
        {
            switch (Mode)
            {
                case InputMode.Default: // Start polling for cell
                    Mode = InputMode.Point;
                    cell = null;
                    return Mode;
                case InputMode.Cancelling: // Stop polling for cell
                    Mode = InputMode.Default;
                    cell = null;
                    return Mode;
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
                    }
                    return Mode;
                default:
                    throw new System.Exception(
                        "PlayerControl is in an illegal input mode.");
            }
        }
    }
}

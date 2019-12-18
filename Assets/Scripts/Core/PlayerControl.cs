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

    public enum InputType
    {
        None,
        Direction,
        Wait,
        Use,
        Pickup,
        Drop
    }

    public struct InputMessage
    {
        public readonly InputType type;
        public readonly Vector2Int vector;
        public readonly bool ctrl;
        public readonly bool shift;
        public readonly bool alt;

        public InputMessage(InputType type, Vector2Int vector, bool ctrl,
            bool shift, bool alt)
        {
            this.type = type;
            this.vector = vector;
            this.ctrl = ctrl;
            this.shift = shift;
            this.alt = alt;
        }
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

            InputType type = InputType.None;
            Vector2Int inputVector = Vector2Int.zero;

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
                type = InputType.Direction;
                inputVector = Vector2Int.up;
            }
            else if (Input.GetButtonDown("Down"))
            {
                type = InputType.Direction;
                inputVector = Vector2Int.down;
            }
            else if (Input.GetButtonDown("Left"))
            {
                type = InputType.Direction;
                inputVector = Vector2Int.left;
            }
            else if (Input.GetButtonDown("Right"))
            {
                type = InputType.Direction;
                inputVector = Vector2Int.right;
            }
            else if (Input.GetButtonDown("Up Left"))
            {
                type = InputType.Direction;
                inputVector = new Vector2Int(-1, 1);
            }
            else if (Input.GetButtonDown("Up Right"))
            {
                type = InputType.Direction;
                inputVector = new Vector2Int(1, 1);
            }
            else if (Input.GetButtonDown("Down Left"))
            {
                type = InputType.Direction;
                inputVector = new Vector2Int(-1, -1);
            }
            else if (Input.GetButtonDown("Down Right"))
            {
                type = InputType.Direction;
                inputVector = new Vector2Int(1, -1);
            }
            else if (Input.GetButtonDown("Wait"))
                type = InputType.Wait;
            else if (Input.GetButtonDown("Use"))
                type = InputType.Use;
            else if (Input.GetButtonDown("Pickup"))
                type = InputType.Pickup;
            else if (Input.GetButtonDown("Inventory"))
                type = InputType.Drop;

            InputMessage msg = new InputMessage(type, inputVector, false,
                false, false);
            SendInput(msg);
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

        private void SendInput(InputMessage msg)
        {
            switch (msg.type)
            {
                case InputType.Direction:
                    playerActor.Command = new MoveCommand(PlayerEntity, msg.vector);
                    break;
                case InputType.Wait:
                    playerActor.Command = new WaitCommand(PlayerEntity);
                    break;
                case InputType.Use:
                    playerActor.Command = new UseItemCommand(PlayerEntity,
                        PlayerEntity.GetComponent<Inventory>().Items[0]);
                    break;
                case InputType.Pickup:
                    playerActor.Command = new PickupCommand(PlayerEntity);
                    break;
                case InputType.Drop:
                    playerActor.Command = new DropCommand(PlayerEntity);
                    break;
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

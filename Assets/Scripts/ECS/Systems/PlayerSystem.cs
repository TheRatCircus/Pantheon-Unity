// PlayerSystem.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;
using Cursor = Pantheon.UI.Cursor;

namespace Pantheon.ECS.Systems
{
    /// <summary>
    /// Player input handler.
    /// </summary>
    public sealed class PlayerSystem : ComponentSystem
    {
        public bool SendingToActor { get; set; } = true;

        private Cursor cursor;

        public PlayerSystem(EntityManager mgr, Cursor cursor) : base(mgr)
        {
            this.cursor = cursor;
        }

        public override void UpdateComponents()
        {
            if (!SendingToActor)
                return;

            if (!Input.anyKeyDown)
                return;

            InputType type = InputType.None;
            Vector2Int inputVector = Vector2Int.zero;
            
            // Set automove path
            if (Input.GetMouseButtonDown(0))
            {
                Entity player = mgr.Player;
                Position pos = player.GetComponent<Position>();
                player.GetComponent<Player>().AutoMovePath = pos.Level.GetPathTo(
                    pos.Cell, cursor.HoveredCell);
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

            InputMessage msg = new InputMessage(type, inputVector, false,
                false, false);
            mgr.Player.GetComponent<Player>().SendInput(msg);
        }
    }

    public enum InputType
    {
        None,
        Direction,
        Wait
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
}

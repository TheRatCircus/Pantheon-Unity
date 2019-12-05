// Player.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
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

    public sealed class Player : MonoBehaviour
    {
        private static UI.Cursor cursor;

        private Actor actor;

        public List<Cell> AutoMovePath { get; set; }
            = new List<Cell>();

        private void OnEnable()
        {
            actor = GetComponent<Actor>();
        }

        private void Update()
        {
            if (!Input.anyKeyDown)
                return;

            InputType type = InputType.None;
            Vector2Int inputVector = Vector2Int.zero;

            // Set automove path
            if (Input.GetMouseButtonDown(0))
            {
                AutoMovePath = actor.Level.GetPathTo(actor.Cell, cursor.HoveredCell);
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
            SendInput(msg);
        }

        private void SendInput(InputMessage msg)
        {
            switch (msg.type)
            {
                case InputType.Direction:
                    actor.Command = new MoveCommand(actor, msg.vector,
                        TurnScheduler.TurnTime);
                    break;
                case InputType.Wait:
                    actor.Command = new WaitCommand(actor);
                    break;
            }
        }

        public static void Init(UI.Cursor cursor)
        {
            Player.cursor = cursor;
        }
    }
}

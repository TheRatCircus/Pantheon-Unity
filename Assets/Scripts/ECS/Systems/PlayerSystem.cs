// PlayerSystem.cs
// Jerome Martina

using System;
using UnityEngine;

namespace Pantheon.ECS.Systems
{
    /// <summary>
    /// Player input handler.
    /// </summary>
    public sealed class PlayerSystem : ComponentSystem
    {
        public Action<InputMessage> InputMessageEvent;

        public PlayerSystem(EntityManager mgr) : base(mgr) { }

        public override void UpdateComponents()
        {
            InputType axis = InputType.None;
            Vector2Int inputVector = Vector2Int.zero;

            if (!Input.anyKeyDown)
                return;

            if (Input.GetButtonDown("Up"))
            {
                axis = InputType.Direction;
                inputVector = Vector2Int.up;
            }
            else if (Input.GetButtonDown("Down"))
            {
                axis = InputType.Direction;
                inputVector = Vector2Int.down;
            }
            else if (Input.GetButtonDown("Left"))
            {
                axis = InputType.Direction;
                inputVector = Vector2Int.left;
            }
            else if (Input.GetButtonDown("Right"))
            {
                axis = InputType.Direction;
                inputVector = Vector2Int.right;
            }
            else if (Input.GetButtonDown("Up Left"))
            {
                axis = InputType.Direction;
                inputVector = new Vector2Int(-1, 1);
            }
            else if (Input.GetButtonDown("Up Right"))
            {
                axis = InputType.Direction;
                inputVector = new Vector2Int(1, 1);
            }
            else if (Input.GetButtonDown("Down Left"))
            {
                axis = InputType.Direction;
                inputVector = new Vector2Int(-1, -1);
            }
            else if (Input.GetButtonDown("Down Right"))
            {
                axis = InputType.Direction;
                inputVector = new Vector2Int(1, -1);
            }
            else if (Input.GetButtonDown("Wait"))
                axis = InputType.Wait;

            InputMessage msg = new InputMessage(axis, inputVector, false,
                false, false);
            InputMessageEvent?.Invoke(msg);
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

        public InputMessage(InputType axis, Vector2Int vector, bool ctrl,
            bool shift, bool alt)
        {
            this.type = axis;
            this.vector = vector;
            this.ctrl = ctrl;
            this.shift = shift;
            this.alt = alt;
        }
    }
}

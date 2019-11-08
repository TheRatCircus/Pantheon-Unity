// PlayerSystem.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS.Systems
{
    public sealed class PlayerSystem : MonoBehaviour
    {
        private Player player;

        private void Update()
        {
            InputAxis axis = InputAxis.None;

            if (Input.GetButtonDown("Up"))
                axis = InputAxis.Up;
            else if (Input.GetButtonDown("Down"))
                axis = InputAxis.Down;
            else if (Input.GetButtonDown("Left"))
                axis = InputAxis.Left;
            else if (Input.GetButtonDown("Right"))
                axis = InputAxis.Right;
            else if (Input.GetButtonDown("Up Left"))
                axis = InputAxis.UpLeft;
            else if (Input.GetButtonDown("Up Right"))
                axis = InputAxis.UpRight;
            else if (Input.GetButtonDown("Down Left"))
                axis = InputAxis.DownLeft;
            else if (Input.GetButtonDown("Down Right"))
                axis = InputAxis.DownRight;
            else if (Input.GetButtonDown("Wait"))
                axis = InputAxis.Wait;

            InputMessage msg = new InputMessage(axis, false, false, false);
        }
    }

    public enum InputAxis
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        Wait
    }

    public struct InputMessage
    {
        public readonly InputAxis axis;
        public readonly bool ctrl;
        public readonly bool shift;
        public readonly bool alt;

        public InputMessage(InputAxis axis, bool ctrl, bool shift, bool alt)
        {
            this.axis = axis;
            this.ctrl = ctrl;
            this.shift = shift;
            this.alt = alt;
        }
    }
}

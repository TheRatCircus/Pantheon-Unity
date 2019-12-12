// PlayerControl.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Core
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

    public sealed class PlayerControl : MonoBehaviour
    {
        [SerializeField] private UI.Cursor cursor = default;

        private EntityManager mgr;

        public bool SendingInput { get; set; } = true;

        public HashSet<Entity> VisibleActors { get; private set; }
            = new HashSet<Entity>();
        public List<Cell> AutoMovePath { get; set; }
            = new List<Cell>();

        public void Initialize(EntityManager mgr)
        {
            this.mgr = mgr;
        }

        private void Update()
        {
            if (!SendingInput)
                return;

            if (!Input.anyKeyDown)
                return;

            InputType type = InputType.None;
            Vector2Int inputVector = Vector2Int.zero;

            // Set automove path
            if (Input.GetMouseButtonDown(0))
            {
                AutoMovePath = mgr.LevelOfPlayer().GetPathTo(mgr.CellOfPlayer(), cursor.HoveredCell);
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
                    if (mgr.PlayerLocation().Walk(msg.vector))
                        mgr.PlayerActor.ActionCost = TurnScheduler.TurnTime;
                    break;
                case InputType.Wait:
                    mgr.PlayerActor.ActionCost = TurnScheduler.TurnTime;
                    break;
            }
        }
    }
}

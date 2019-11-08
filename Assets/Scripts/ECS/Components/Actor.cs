// Actor.cs
// Jerome Martina

using Pantheon.Actions;
using Pantheon.ECS.Messages;
using Pantheon.ECS.Systems;
using UnityEngine;

namespace Pantheon.ECS.Components
{
    public enum ActorControl
    {
        None,
        AI,
        Player
    }

    [System.Serializable]
    public sealed class Actor : BaseComponent
    {
        [SerializeField] private int speed = -1;
        public int Speed => speed;
        [SerializeField] private int energy = -1;
        public int Energy { get => energy; set => energy = value; }

        public Command Command { get; set; } = null;

        public ActorControl ActorControl { get; set; } = ActorControl.None;
        public bool PlayerControlled => ActorControl == ActorControl.Player;
        public bool AIControlled => ActorControl == ActorControl.AI;

        /// <summary>
        /// Consume this actor's action.
        /// </summary>
        /// <returns></returns>
        public int Act()
        {
            if (Command != null)
            {
                int cost = Command.DoAction();
                Command = null;
                return cost;
            }
            else return -1;
        }

        /// <summary>
        /// Request that nextAction be set by an AI component.
        /// </summary>
        public void RequestAICommand()
            => Message<AI>(new AIRequestMessage(this));
    }

    [System.Serializable]
    public sealed class Player : BaseComponent
    {
        private Actor actor;

        public void SendInput(InputMessage msg)
        {
            switch (msg.axis)
            {
                case InputAxis.Up:
                    {
                        break;
                    }
                case InputAxis.Right:
                    {
                        break;
                    }
                case InputAxis.Down:
                    {
                        break;
                    }
                case InputAxis.Left:
                    {
                        break;
                    }
                case InputAxis.UpRight:
                    {
                        break;
                    }
                case InputAxis.DownRight:
                    {
                        break;
                    }
                case InputAxis.DownLeft:
                    {
                        break;
                    }
                case InputAxis.UpLeft:
                    {
                        break;
                    }
                default:
                    break;
            }
        }
    }

    [System.Serializable]
    public sealed class AI : BaseComponent
    {
        private Actor actor;

        private void Act()
        {

        }
    }
}

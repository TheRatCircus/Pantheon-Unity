// Actor.cs
// Jerome Martina

using Pantheon.Commands;
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
                int cost = Command.Execute();
                Command = null;
                return cost;
            }
            else return -1;
        }

        /// <summary>
        /// Request that the next command be set by an AI component.
        /// </summary>
        public void RequestAICommand()
            => Message<AI>(new AIRequestMessage(this));

        public bool HostileTo(Entity other)
        {
            if (PlayerControlled)
            {
                if (other.HasComponent<AI>())
                    return true;
                else
                    return false;
            }
            else if (AIControlled)
            {
                if (other.HasComponent<Player>())
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }

    [System.Serializable]
    public sealed class Player : BaseComponent
    {
        private Entity entity;
        private Actor actor;

        public void SendInput(InputMessage msg)
        {
            switch (msg.type)
            {
                case InputType.Direction:
                    actor.Command = new MoveCommand(entity, msg.vector,
                        ActorSystem.TurnTime);
                    break;
                case InputType.Wait:
                    actor.Command = new WaitCommand(entity);
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

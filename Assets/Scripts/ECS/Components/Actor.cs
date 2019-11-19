// Actor.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.ECS.Messages;
using Pantheon.ECS.Systems;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.ECS.Components
{
    public enum ActorControl
    {
        None,
        AI,
        Player
    }

    [Serializable]
    public sealed class Actor : BaseComponent
    {
        [SerializeField] private int speed = -1;
        public int Speed => speed;
        [SerializeField] private int energy = -1;
        public int Energy { get => energy; set => energy = value; }

        [NonSerialized] private Command command = null;
        public Command Command { get => command; set => command = value; }

        [SerializeField] public ActorControl control = default;
        public ActorControl Control
        {
            get => control;
            set => control = value;
        }

        public bool PlayerControlled => Control == ActorControl.Player;
        public bool AIControlled => Control == ActorControl.AI;

        public Actor(int speed, int energy, ActorControl control)
        {
            this.speed = speed;
            this.energy = energy;
            this.control = control;
        }

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

        public override BaseComponent Clone()
            => new Actor(speed, energy, control);

        public override string ToString()
            => $"Controller: {Control} Command: {Command}";
    }

    [Serializable]
    public sealed class Player : BaseComponent
    {
        public Entity Entity { get; set; }
        public Actor Actor { get; set; }
        public List<Cell> AutoMovePath { get; set; }
            = new List<Cell>();

        public void SendInput(InputMessage msg)
        {
            switch (msg.type)
            {
                case InputType.Direction:
                    Actor.Command = new MoveCommand(Entity, msg.vector,
                        ActorSystem.TurnTime);
                    break;
                case InputType.Wait:
                    Actor.Command = new WaitCommand(Entity);
                    break;
            }
        }

        public override BaseComponent Clone() => new Player();
    }

    [Serializable]
    public sealed class AI : BaseComponent
    {
        public Entity Target { get; set; }

        public override BaseComponent Clone() => new AI();
    }
}

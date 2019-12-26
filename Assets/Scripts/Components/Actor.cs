// Actor.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Commands;
using Pantheon.Utils;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Pantheon.Components
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActorControl
    {
        None,
        AI,
        Player
    }

    [Serializable]
    public sealed class Actor : EntityComponent
    {
        [SerializeField] private int speed = -1;
        public int Speed
        {
            get => speed;
            set { speed = value; SpeedChangedEvent?.Invoke(this); }
        }
        [JsonIgnore] private int energy;
        public int Energy
        {
            get => energy;
            set
            {
                energy = value;
                EnergyChangedEvent?.Invoke(this);
            }
        }
        [JsonIgnore]
        [NonSerialized]
        private ActorCommand command;
        public ActorCommand Command { get => command; set => command = value; }

        public event Action AIDecisionEvent;

        [SerializeField] private ActorControl control = default;
        [JsonIgnore] public ActorControl Control
        {
            get => control;
            set => control = value;
        }

        public event Action<Actor> EnergyChangedEvent;
        public event Action<Actor> SpeedChangedEvent;

        public Actor() => Energy = speed;

        [JsonConstructor]
        public Actor(int speed, ActorControl control) : this()
        {
            this.speed = speed;
            this.control = control;
        }

        public int Act()
        {
            if (Control == ActorControl.AI)
            {
                AIDecisionEvent?.Invoke();
            }

            if (Command != null)
            {
                CommandResult result = Command.Execute(out int cost);
                if (result != CommandResult.InProgress)
                    Command = null;
                return cost;
            }
            else
                return -1;
        }

        public bool HostileTo(Actor other)
        {
            if (other == this) // Actor probably not hostile to itself
                return false;

            if (Entity.TryGetComponent(out AI ai))
            {
                // If other is not this actor's master, be hostile to it
                if (ai.Strategy is ThrallFollowStrategy tfs)
                    return tfs.Master != other;
                else
                    return true;
            }
            else if (Control == ActorControl.Player)
            {
                AI otherAi = other.Entity.GetComponent<AI>();

                // Don't be hostile to a thrall
                if (otherAi.Strategy is ThrallFollowStrategy tfs)
                    return tfs.Master != this;
                else
                    return true;
            }
            else // This actor is uncontrolled
                return false;
        }

        public override EntityComponent Clone(bool full)
            => new Actor(speed, control);

        public override string ToString()
        {
            return $"Actor ({Control}) - Energy: {Energy}";
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctxt)
        {
            Helpers.ClearNonSerializableDelegates(ref EnergyChangedEvent);
            Helpers.ClearNonSerializableDelegates(ref SpeedChangedEvent);
        }
    }
}

// Actor.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Commands;
using System;
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
                int cost = Command.Execute();
                Command = null;
                return cost;
            }
            else
                return -1;
        }

        public bool HostileTo(Actor other) => true;

        public override EntityComponent Clone() => new Actor(speed, control);

        public override string ToString()
        {
            return $"Actor ({Control}) - Energy: {Energy}";
        }
    }
}

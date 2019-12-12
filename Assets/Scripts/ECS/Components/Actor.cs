// Actor.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using UnityEngine;

namespace Pantheon.ECS.Components
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
        public int Speed { get => speed; set => speed = value; }
        [JsonIgnore] public int Energy { get; set; }
        [JsonIgnore] public int ActionCost { get; set; } = -1;

        [SerializeField] private ActorControl control = default;
        [JsonIgnore] public ActorControl Control
        {
            get => control;
            set => control = value;
        }

        public Actor() => Energy = speed;

        [JsonConstructor]
        public Actor(int speed, ActorControl control) : this()
        {
            this.speed = speed;
            this.control = control;
        }

        public int Act()
        {
            if (ActionCost < 0)
                return ActionCost;
            else // Consume action
            {
                int ret = ActionCost;
                ActionCost = -1; // Reset
                return ret;
            }
        }

        public bool HostileTo(Actor other) => true;

        public override EntityComponent Clone() => new Actor(speed, control);

        public override string ToString()
        {
            return $"Actor ({Control}) - Energy: {Energy}";
        }
    }
}

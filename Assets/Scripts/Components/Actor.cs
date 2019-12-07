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
        public int Speed { get => speed; set => speed = value; }
        [JsonIgnore] public int Energy { get; set; }
        [JsonIgnore]
        [NonSerialized]
        private ActorCommand command;
        public ActorCommand Command { get => command; set => command = value; }

        [SerializeField] public ActorControl control = default;
        [JsonIgnore] public ActorControl Control
        {
            get => control;
            set => control = value;
        }

        private void Awake()
        {
            Energy = speed;
        }

        public int Act()
        {
            if (Control == ActorControl.AI)
            {

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

        public override string ToString()
        {
            return $"Actor - Speed: {Speed} Command: {Command}";
        }
    }
}

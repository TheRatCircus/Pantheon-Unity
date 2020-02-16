// Actor.cs
// Jerome Martina

#define DEBUG_ACTOR
#undef DEBUG_ACTOR

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Commands;
using Pantheon.Commands.Actor;
using System;
using System.Diagnostics;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActorControl : byte
    {
        None,
        AI,
        Player
    }

    [Serializable]
    public sealed class Actor : EntityComponent, IEntityDependentComponent
    {
        public int Progress { get; set; }
        public int Threshold { get; set; }

        [JsonIgnore]
        [NonSerialized]
        private ActorCommand command;
        public ActorCommand Command { 
            get => command;
            set 
            { 
                command = value;
                if (command != null)
                    Threshold = command.Cost;
                else
                    Threshold = 0;
            }
        }
        public ActorControl Control { get; set; } = default;

        public static bool PlayerControlled(Entity entity)
        {
            if (entity.TryGetComponent(out Actor actor))
                return actor.Control == ActorControl.Player;
            else
                return false;
        }

        public int Act()
        {
            if (Command != null)
            {
                CommandResult result = Command.Execute();
                int cost = Command.Cost;
                DebugLogCommand($"{Entity} doing: {Command} ({Command.Cost})");
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
                return true;
            else if (Control == ActorControl.Player)
                return true;
            else // This actor is uncontrolled
                return false;
        }

        public override EntityComponent Clone(bool full)
            => new Actor() { Control = Control };

        public override string ToString() => $"Actor ({Control})";

        [Conditional("DEBUG_ACTOR")]
        private void DebugLogCommand(string msg) => UnityEngine.Debug.Log(msg);

        public void Initialize()
        {
            Entity.DestroyedEvent += delegate ()
            {
                if (Command is TalentCommand tc)
                    tc.UnmarkTargeted();
            };
        }
    }
}

// Command.cs
// Jerome Martina

using Newtonsoft.Json;

namespace Pantheon.Commands
{
    /// <summary>
    /// Base class for all actions in Pantheon's system of commands.
    /// </summary>
    [System.Serializable]
    public abstract class Command
    {
        // Entity responsible for command, if any exists
        [JsonIgnore] public Entity Entity { get; set; }

        public Command(Entity entity) => Entity = entity;
    }

    public enum CommandResult
    {
        Failed,
        Cancelled,
        InProgress,
        Succeeded
    }
}

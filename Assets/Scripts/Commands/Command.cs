// Command.cs
// Jerome Martina

namespace Pantheon.Commands
{
    /// <summary>
    /// Base class for all actions in Pantheon's system of commands.
    /// </summary>
    public abstract class Command
    {
        // Entity responsible for command, if any exists
        public Entity Entity { get; set; }

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

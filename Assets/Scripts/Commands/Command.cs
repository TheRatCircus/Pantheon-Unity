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
        public Entity Entity { get; private set; }

        public Command(Entity entity) => Entity = entity;

        /// <summary>
        /// Carry out this commands's effects.
        /// </summary>
        public abstract void Execute();
    }
}

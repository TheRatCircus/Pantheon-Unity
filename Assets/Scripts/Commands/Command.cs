// Command.cs
// Jerome Martina

using Pantheon.ECS;

namespace Pantheon.Commands
{
    /// <summary>
    /// Base class for all actions in Pantheon's system of commands.
    /// </summary>
    public abstract class Command
    {
        // Callback which can be run after completion of an action
        public delegate void OnConfirm();
        public OnConfirm onConfirm = null;

        public Entity Entity { get; private set; } = null;

        public Command(Entity entity) => Entity = entity;

        /// <summary>
        /// Carry out this commands's effects.
        /// </summary>
        /// <returns>The energy cost of this action.</returns>
        public abstract int Execute();
    }
}

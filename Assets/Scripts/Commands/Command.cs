// Command.cs
// Jerome Martina

namespace Pantheon.Commands
{
    /// <summary>
    /// Base class for all actions in Pantheon's system of commands.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Carry out this commands's effects.
        /// </summary>
        public abstract void Execute();
    }
}

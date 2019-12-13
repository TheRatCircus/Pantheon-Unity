// ActorCommand.cs
// Jerome Martina

namespace Pantheon.Commands
{
    /// <summary>
    /// Base class for actor actions (comes with an energy cost).
    /// </summary>
    public abstract class ActorCommand : Command
    {
        public int Cost { get; private set; } // XXX: Doesn't do anything right now

        public ActorCommand(Entity entity, int cost) : base(entity)
        {
            Cost = cost;
        }

        /// <summary>
        /// Carry out this commands's effects.
        /// </summary>
        /// <returns>The energy cost this action incurs.</returns>
        public abstract int Execute();
    }
}


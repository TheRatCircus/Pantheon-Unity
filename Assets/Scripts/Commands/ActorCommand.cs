// ActorCommand.cs
// Jerome Martina

namespace Pantheon.Commands
{
    /// <summary>
    /// Base class for actor actions (comes with an energy cost).
    /// </summary>
    public abstract class ActorCommand : Command
    {
        public ActorCommand(Entity entity) : base(entity) { }

        public abstract CommandResult Execute(out int cost);
    }
}


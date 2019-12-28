// NonActorCommand.cs
// Jerome Martina

namespace Pantheon.Commands.NonActor
{
    /// <summary>
    /// A command not directly performed by an actor-driven entity.
    /// </summary>
    public abstract class NonActorCommand : Command
    {
        public NonActorCommand(Entity entity) : base(entity) { }

        public abstract CommandResult Execute();
    }
}

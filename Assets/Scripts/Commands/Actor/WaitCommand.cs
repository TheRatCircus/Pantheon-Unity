// WaitCommand.cs
// Jerome Martina

using Pantheon.Core;

namespace Pantheon.Commands.Actor
{
    public sealed class WaitCommand : ActorCommand
    {
        public WaitCommand(Entity entity)
            : base(entity) => Cost = TurnScheduler.TurnTime;

        public override CommandResult Execute()
        {
            return CommandResult.Succeeded;
        }

        public override string ToString()
            => $"{Entity} waiting for {Cost} ticks.";
    }
}

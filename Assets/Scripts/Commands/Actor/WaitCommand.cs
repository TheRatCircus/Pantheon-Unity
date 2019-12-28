// WaitCommand.cs
// Jerome Martina

using Pantheon.Core;

namespace Pantheon.Commands.Actor
{
    public sealed class WaitCommand : ActorCommand
    {
        public WaitCommand(Entity entity)
            : base(entity) { }

        public override CommandResult Execute(out int cost)
        {
            cost = TurnScheduler.TurnTime;
            return CommandResult.Succeeded;
        }

        public override string ToString()
        {
            return $"{Entity} waiting for 100 ticks.";
        }
    }
}

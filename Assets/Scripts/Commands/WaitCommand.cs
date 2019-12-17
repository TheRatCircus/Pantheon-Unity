// WaitCommand.cs
// Jerome Martina

using Pantheon.Core;

namespace Pantheon.Commands
{
    public sealed class WaitCommand : ActorCommand
    {
        public WaitCommand(Entity entity) : base(entity) { }

        public override int Execute() => TurnScheduler.TurnTime;

        public override string ToString()
        {
            return $"{Entity} waiting for 100 ticks.";
        }
    }
}

// WaitCommand.cs
// Jerome Martina

namespace Pantheon.Commands
{
    public sealed class WaitCommand : ActorCommand
    {
        private int waitTime = TurnScheduler.TurnTime;

        public WaitCommand(Entity entity, int waitTime = TurnScheduler.TurnTime)
            : base(entity, waitTime)
            => this.waitTime = waitTime;

        public override int Execute() => waitTime;
    }
}

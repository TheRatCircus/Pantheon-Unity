// WaitCommand.cs
// Jerome Martina

namespace Pantheon.Commands
{
    public sealed class WaitCommand : ActorCommand
    {
        public WaitCommand(Entity entity, int waitTime = 100)
            : base(entity, waitTime) { }

        public override void Execute() { }
    }
}

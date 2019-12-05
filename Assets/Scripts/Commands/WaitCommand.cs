// WaitCommand.cs
// Jerome Martina

namespace Pantheon.Commands
{
    public sealed class WaitCommand : ActorCommand
    {
        public WaitCommand(Actor actor, int waitTime = 100)
            : base(actor, waitTime) { }

        public override void Execute() { }
    }
}

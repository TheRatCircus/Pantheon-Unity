// WaitCommand.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.ECS.Systems;

namespace Pantheon.Commands
{
    public sealed class WaitCommand : Command
    {
        public WaitCommand(Entity entity) : base(entity) { }

        public override int Execute() => ActorSystem.TurnTime;
    }
}

// MeleeCommand.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Commands
{
    public sealed class MeleeCommand : ActorCommand
    {
        public Cell Target { get; private set; }

        public MeleeCommand(Entity entity, int attackTime, Cell target)
            : base(entity, attackTime)
        {
            Target = target;
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}

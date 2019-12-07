// MeleeCommand.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Commands
{
    public sealed class MeleeCommand : ActorCommand
    {
        private Cell target;
        
        public MeleeCommand(Entity entity, int attackTime, Cell target)
            : base(entity, attackTime)
        {
            this.target = target;
        }

        public override int Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}

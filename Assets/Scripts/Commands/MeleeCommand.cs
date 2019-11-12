// MeleeCommand.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.World;

namespace Pantheon.Commands
{
    public sealed class MeleeCommand : Command
    {
        public Cell Target { get; private set; }

        public MeleeCommand(Entity attacker, Cell target) : base(attacker)
        {
            Target = target;
        }

        public override int Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}

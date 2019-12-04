// MeleeCommand.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Commands
{
    public sealed class MeleeCommand : Command
    {
        public Cell Target { get; private set; }

        public MeleeCommand(Cell target) : base()
        {
            Target = target;
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}

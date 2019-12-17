// UseItemCommand.cs
// Jerome Martina

using Pantheon.Components;

namespace Pantheon.Commands
{
    public sealed class UseItemCommand : ActorCommand
    {
        private Entity item;

        public UseItemCommand(Entity user, Entity item)
            : base(user) => this.item = item;

        public override int Execute()
        {
            if (!item.TryGetComponent(out OnUse onUse))
                return -1;
            else
                return onUse.Invoke(Entity);
        }
    }
}

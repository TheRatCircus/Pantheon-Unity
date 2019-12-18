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

        public override CommandResult Execute(out int cost)
        {
            if (!item.TryGetComponent(out OnUse onUse))
            {
                cost = -1;
                return CommandResult.Failed;
            }
            else
            {
                CommandResult result = onUse.Invoke(Entity);
                if (result == CommandResult.InProgress ||
                    result == CommandResult.Failed ||
                    result == CommandResult.Cancelled)
                    cost = -1;
                else
                    cost = onUse.UseTime;
                return result;
            }
        }
    }
}

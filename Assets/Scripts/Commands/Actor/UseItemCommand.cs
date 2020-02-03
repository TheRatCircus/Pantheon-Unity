// UseItemCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;

namespace Pantheon.Commands.Actor
{
    public sealed class UseItemCommand : ActorCommand
    {
        private readonly Entity item;

        public UseItemCommand(Entity user, Entity item) : base(user)
        {
            if (!item.TryGetComponent(out OnUse onUse))
            {
                Cost = -1;
            }
            else
            {
                Cost = Cost = onUse.UseTime;
            }  
            this.item = item;
        }

        public override CommandResult Execute()
        {
            if (!item.TryGetComponent(out OnUse onUse))
            {
                return CommandResult.Failed;
            }
            else
            {
                CommandResult result = onUse.Invoke(Entity);
                return result;
            }
        }
    }
}

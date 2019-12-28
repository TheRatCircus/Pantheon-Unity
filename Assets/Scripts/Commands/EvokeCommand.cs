// EvokeCommand.cs
// Jerome Martina

using Pantheon.Components;

namespace Pantheon.Commands
{
    public sealed class EvokeCommand : ActorCommand
    {
        private Entity item;

        public EvokeCommand(Entity entity, Entity evocable) : base(entity)
        {
            item = evocable;
        }

        public override CommandResult Execute(out int cost)
        {
            if (!item.TryGetComponent(out Evocable evoc))
            {
                cost = -1;
                return CommandResult.Failed;
            }
            else
            {
                CommandResult result = evoc.Evoke(Entity, 0);
                if (result == CommandResult.InProgress ||
                    result == CommandResult.Failed ||
                    result == CommandResult.Cancelled)
                    cost = -1;
                else
                    cost = evoc.EvokeTime;
                return result;
            }
        }
    }
}

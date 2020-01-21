// EvokeCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Commands.Actor
{
    public sealed class EvokeCommand : ActorCommand
    {
        private Entity item;

        public Cell Cell { get; set; }
        public List<Cell> Line { get; set; }
        public List<Cell> Path { get; set; }

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
                CommandResult result;
                if (Cell != null && Line != null && Path != null)
                    result = evoc.Evoke(Entity, 0, Cell, Line, Path);
                else
                    result = evoc.Evoke(Entity, 0);

                if (result != CommandResult.Succeeded)
                    cost = -1;
                else
                    cost = evoc.EvokeTime;

                return result;
            }
        }
    }
}

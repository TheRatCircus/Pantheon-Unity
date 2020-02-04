// TalentCommand.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Commands.Actor
{
    using Actor = Components.Entity.Actor;

    public sealed class TalentCommand : ActorCommand
    {
        private readonly Talent talent;
        private readonly Cell target;

        public TalentCommand(Entity entity, Talent talent, Cell target)
            : base(entity)
        {
            this.talent = talent;
            this.target = target;

            Cost = talent.Time;

            if (!Actor.PlayerControlled(Entity))
            {
                HashSet<Cell> cells = new HashSet<Cell>();
                cells.AddMany(talent.Behaviour.GetTargetedCells(Entity, target));
                foreach (Cell c in cells)
                    Locator.Scheduler.TargetCell(c.Position);
            }
        }

        public override CommandResult Execute()
        {
            if (!Actor.PlayerControlled(Entity))
            {
                HashSet<Cell> cells = new HashSet<Cell>();
                cells.AddMany(talent.Behaviour.GetTargetedCells(Entity, target));
                foreach (Cell c in cells)
                    Locator.Scheduler.UntargetCell(c.Position);
            }
            return talent.Cast(Entity, target);
        }
    }
}

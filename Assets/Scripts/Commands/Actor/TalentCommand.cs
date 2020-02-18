// TalentCommand.cs
// Jerome Martina

using Pantheon.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    using Actor = Components.Entity.Actor;

    public sealed class TalentCommand : ActorCommand
    {
        private readonly Talent talent;
        private readonly Vector2Int target;

        public TalentCommand(Entity entity, Talent talent, Vector2Int target)
            : base(entity)
        {
            this.talent = talent;
            this.target = target;

            if (!Actor.PlayerControlled(Entity))
            {
                Cost = talent.NPCTime;
                HashSet<Vector2Int> cells = new HashSet<Vector2Int>();
                cells.AddMany(talent.GetTargetedCells(Entity, target));
                foreach (Vector2Int c in cells)
                    if (entity.Level.Visible(c.x, c.y))
                        Locator.Scheduler.MarkCell(c);
            }
            else
                Cost = talent.PlayerTime;
        }

        public override CommandResult Execute()
        {
            if (!Actor.PlayerControlled(Entity))
                UnmarkTargeted();
            return talent.Cast(Entity, target);
        }

        public void UnmarkTargeted()
        {
            HashSet<Vector2Int> cells = new HashSet<Vector2Int>();
            cells.AddMany(talent.GetTargetedCells(Entity, target));
            foreach (Vector2Int c in cells)
                Locator.Scheduler.UnmarkCell(c);
        }
    }
}

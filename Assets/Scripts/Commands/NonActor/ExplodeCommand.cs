// ExplodeCommand.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Commands.NonActor
{
    public enum ExplosionPattern
    {
        Point,
        Line,
        Path,
        Flood,
        Square
    }

    public sealed class ExplodeCommand : NonActorCommand,
        ICellTargetedCommand, ILineTargetedCommand
    {
        private Damage[] damages;
        private ExplosionPattern pattern;
        private GameObject prefab;

        public Cell Cell { get; set; }
        public List<Cell> Line { get; set; }

        public ExplodeCommand(Entity entity, GameObject prefab,
            ExplosionPattern pattern, params Damage[] damages) : base(entity)
        {
            this.damages = damages;
            this.prefab = prefab;
            this.pattern = pattern;
        }

        public override CommandResult Execute()
        {
            // Fall back on assumption that entity itself is exploding
            if (Cell == null)
                Cell = Entity.Cell;

            switch (pattern)
            {
                case ExplosionPattern.Point:
                    {
                        GameObject explObj = Object.Instantiate(
                        prefab, Cell.Position.ToVector3(), new Quaternion(), null);
                        PointExplosion expl = explObj.GetComponent<PointExplosion>();
                        expl.Initialize(Entity, Cell);
                        expl.Fire(damages);
                        Object.Destroy(explObj, 5f);
                        break;
                    }
                case ExplosionPattern.Line:
                    if (Line == null)
                        return CommandResult.Failed;

                    // Exclude origin cell
                    for (int i = 1; i < Line.Count; i++)
                    {
                        Cell c = Line[i];
                        GameObject explObj = Object.Instantiate(
                        prefab, c.Position.ToVector3(), new Quaternion(), null);
                        PointExplosion expl = explObj.GetComponent<PointExplosion>();
                        expl.Initialize(Entity, c);
                        expl.Fire(damages);
                        Object.Destroy(explObj, 5f);
                    }

                    Line = null;
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            return CommandResult.Succeeded;
        }
    }
}

// ExplodeCommand.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Commands.NonActor
{
    [JsonConverter(typeof(StringEnumConverter))]
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
        public Damage[] Damages { get; set; }
        public ExplosionPattern Pattern { get; set; }
        public GameObject Prefab { get; set; }
        public AudioClip Sound { get; set; }

        public Cell Cell { get; set; }
        public List<Cell> Line { get; set; }

        public ExplodeCommand(Entity entity) : base(entity) { }

        public override CommandResult Execute()
        {
            // Fall back on assumption that entity itself is exploding
            if (Cell == null)
                Cell = Entity.Cell;

            switch (Pattern)
            {
                case ExplosionPattern.Point:
                    {
                        GameObject explObj = Object.Instantiate(
                        Prefab, Cell.Position.ToVector3(), new Quaternion(), null);
                        PointExplosion expl = explObj.GetComponent<PointExplosion>();
                        expl.Initialize(Entity, Cell);
                        expl.Fire(Damages);
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
                        Prefab, c.Position.ToVector3(), new Quaternion(), null);
                        PointExplosion expl = explObj.GetComponent<PointExplosion>();
                        expl.Initialize(Entity, c);
                        expl.Fire(Damages);
                        Object.Destroy(explObj, 5f);
                    }

                    Line = null;
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            AudioSource.PlayClipAtPoint(Sound, Cell.Position.ToVector3());
            return CommandResult.Succeeded;
        }
    }
}

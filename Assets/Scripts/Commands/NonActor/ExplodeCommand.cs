// ExplodeCommand.cs
// Jerome Martina

using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Commands.NonActor
{
    [System.Serializable]
    public sealed class ExplodeCommand : NonActorCommand,
        ICellTargetedCommand, ILineTargetedCommand
    {
        public Damage[] Damages { get; set; }
        public ExplosionPattern Pattern { get; set; }
        public GameObject Prefab { get; set; }
        public AudioClip Sound { get; set; }

        public Vector2Int Cell { get; set; }
        public Line Line { get; set; }

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
                        Prefab, Cell.ToVector3(), Quaternion.identity, null);
                        Explosion expl = explObj.GetComponent<Explosion>();
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
                        Vector2Int c = Line[i];
                        GameObject explObj = Object.Instantiate(
                        Prefab, c.ToVector3(), Quaternion.identity, null);
                        Explosion expl = explObj.GetComponent<Explosion>();
                        expl.Initialize(Entity, c);
                        expl.Fire(Damages);
                        Object.Destroy(explObj, 5f);
                    }

                    Line = null;
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            Locator.Audio.Buffer(Sound, Cell.ToVector3());
            return CommandResult.Succeeded;
        }
    }
}

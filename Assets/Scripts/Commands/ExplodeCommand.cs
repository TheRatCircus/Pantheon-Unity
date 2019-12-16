// ExplodeCommand.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Commands
{
    public enum ExplosionPattern
    {
        Point,
        Line,
        Path,
        Flood,
        Square
    }

    public sealed class ExplodeCommand : NonActorCommand
    {
        private ExplosionPattern pattern;
        public Cell Origin { get; set; }
        private GameObject prefab;
        
        public ExplodeCommand(Entity entity, GameObject prefab,
            ExplosionPattern pattern) : base(entity)
        {
            this.prefab = prefab;
            this.pattern = pattern;
        }

        public override void Execute()
        {
            // Fall back on assumption that entity itself is exploding
            if (Origin == null)
                Origin = Entity.Cell;

            switch (pattern)
            {
                case ExplosionPattern.Point:
                    GameObject explObj = Object.Instantiate(
                        prefab, Origin.Position.ToVector3(), new Quaternion(), null);
                    PointExplosion expl = explObj.GetComponent<PointExplosion>();
                    expl.Initialize(Entity, Origin);
                    expl.Fire();
                    Object.Destroy(explObj, 5f);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}

// ExplosionEffect.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public class ExplosionEffect : ICellTalentEffect
    {
        public ExplosionPattern Pattern { get; set; }
        public int Range { get; set; } = 1;
        public Damage[] Damages { get; set; }
        public GameObject Prefab { get; set; }
        public AudioClip Sound { get; set; }

        public void Affect(Entity source, Level level, Vector2Int cell)
        {
            switch (Pattern)
            {
                case ExplosionPattern.Square:
                    SquareExplosion(source, level, cell);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public Vector2Int[] GetAffectedCells(Entity source, Level level, Vector2Int cell)
        {
            switch (Pattern)
            {
                case ExplosionPattern.Square:
                    return level.GetSquare(cell, Range).ToArray();
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void SquareExplosion(Entity source, Level level, Vector2Int cell)
        {
            Locator.Audio.Buffer(Sound, cell.ToVector3());
            foreach (Vector2Int v in level.GetSquare(cell, Range))
            {
                GameObject explObj = Object.Instantiate(
                    Prefab, v.ToVector3(), new Quaternion());
                Explosion expl = explObj.GetComponent<Explosion>();
                expl.Initialize(source, v);
                expl.Fire(Damages);
                Object.Destroy(explObj as GameObject, 5f);
            }
        }
    }
}

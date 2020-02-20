// Explosion.cs
// Jerome Martina

using Pantheon.Utils;
using UnityEngine;

namespace Pantheon
{
    public sealed class Explosion : MonoBehaviour
    {
        private Entity source;
        private Vector2Int cell;

        public void Initialize(Entity source, Vector2Int origin)
        {
            this.source = source;
            cell = origin;
        }

        public void Fire(Damage[] damages)
        {
            Entity entity = source.Level.ActorAt(cell);
            if (entity != null)
            {
                Hit hit = new Hit(damages);
                Locator.Log.Send(
                    $"{Verbs.Be(entity)} caught in the blast and take {hit.TotalDamage()} damage!",
                    Color.white);
                entity.TakeHit(source, hit);
            }
        }
    }
}

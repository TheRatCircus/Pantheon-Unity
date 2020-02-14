// PointExplosion.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public sealed class PointExplosion : MonoBehaviour
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
                    $"{Strings.Subject(entity, true)} is caught in the blast!",
                    Color.white);
                entity.TakeHit(source, hit);
            }
        }
    }
}

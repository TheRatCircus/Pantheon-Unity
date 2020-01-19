// PointExplosion.cs
// Jerome Martina

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
            Level level = source.Level;
            Entity actor = level.ActorAt(cell);
            if (actor != null)
            {
                Hit hit = new Hit(damages);
                Locator.Log.Send(
                    $"{actor.ToSubjectString(true)} is caught in the blast!",
                    Color.white);
                actor.TakeHit(source, hit);
            }
        }
    }
}

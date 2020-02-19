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
                // TODO: Message should inform of damage
                // TODO: "You are"
                Hit hit = new Hit(damages);
                Locator.Log.Send(
                    $"{Strings.Subject(entity, true)} is caught in the blast!",
                    Color.white);
                entity.TakeHit(source, hit);
            }
        }
    }
}

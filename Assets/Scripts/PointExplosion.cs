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
        private Cell cell;

        public void Initialize(Entity source, Cell origin)
        {
            this.source = source;
            cell = origin;
        }

        public void Fire(Damage[] damages)
        {
            if (cell.Actor != null)
            {
                Hit hit = new Hit(damages);
                Locator.Log.Send(
                    $"{Strings.Subject(cell.Actor, true)} is caught in the blast!",
                    Color.white);
                cell.Actor.TakeHit(source, hit);
            }
        }
    }
}

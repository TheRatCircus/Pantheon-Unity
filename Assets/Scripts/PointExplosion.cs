// PointExplosion.cs
// Jerome Martina

using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public sealed class PointExplosion : MonoBehaviour
    {
        [SerializeField] private Damage[] damages = default;

        private Entity source;
        private Cell cell;

        public void Initialize(Entity source, Cell origin)
        {
            this.source = source;
            cell = origin;
        }

        public void Fire()
        {
            if (cell.Actor != null)
            {
                Hit hit = new Hit(damages);
                LogLocator.Service.Send(
                    $"{cell.Actor.ToSubjectString(true)} is caught in the blast!",
                    Color.white);
                cell.Actor.TakeHit(source, hit);
            }
        }
    }
}

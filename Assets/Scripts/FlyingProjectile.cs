// FlyingProjectile.cs
// Jerome Martina

using Pantheon.Actions;
using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections;
using UnityEngine;

namespace Pantheon
{
    public sealed class FlyingProjectile : MonoBehaviour
    {
        public string ProjName { get; set; } = "NO_FLYINGPROJ_NAME";
        public Actor Source { get; set; } = null;
        public Cell TargetCell { get; set; } = null;
        public bool Spins { get; set; } = false;
        private Vector3 targetPos;

        public int MinDamage { get; set; } = -1;
        public int MaxDamage { get; set; } = -1;
        public int Accuracy { get; set; } = -1;
        public bool Pierces { get; set; } = false;

        // What happens when the projectile lands?
        public BaseAction OnLandAction;

        private void Start()
        {
            switch (OnLandAction)
            {
                case ExplodeAction a:
                    a.Origin = TargetCell;
                    break;
                case null:
                    break;
                default:
                    throw new System.Exception
                        ($"{OnLandAction.GetType()} cannot be handled.");
            }
            targetPos = Helpers.V2IToV3(TargetCell.Position);
            StartCoroutine(Fly());
        }

        private IEnumerator Fly()
        {
            Game.instance.Lock();
            while (transform.position != targetPos)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPos, .6f);

                if (Spins)
                    transform.Rotate(0, 0, 8, Space.Self);

                yield return new WaitForSeconds(.01f);
            }
            if (TargetCell.Actor != null && OnLandAction == null)
            {
                if (RandomUtils.RangeInclusive(0, 100) < Accuracy)
                {
                    Hit hit = new Hit(MinDamage, MaxDamage);
                    GameLog.Send($"The {ProjName}" +
                    $" {(Pierces ? "punches through" : "hits")} " +
                    $"{Strings.GetSubject(TargetCell.Actor, false)}, " +
                    $"dealing {hit.Damage} damage!");
                    TargetCell.Actor.TakeHit(hit, Source);
                }
            }
            OnLandAction?.DoAction();
            Game.instance.Unlock();
            Destroy(gameObject);
        }
    }
}

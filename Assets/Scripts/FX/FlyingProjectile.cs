// FlyingProjectile.cs
// Jerome Martina

using Pantheon.Actions;
using Pantheon.Actors;
using Pantheon.Components;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections;
using UnityEngine;

namespace Pantheon
{
    public sealed class FlyingProjectile : MonoBehaviour
    {
        public string ProjName { get; set; } = "DEFAULT_FLYINGPROJ_NAME";
        public Actor Source { get; set; } = null;
        public Cell TargetCell { get; set; } = null;
        public bool Spins { get; set; } = false;
        public bool Returns { get; set; } = false;

        // Leave as null if no item is left or returned to sender
        public Item Item { get; set; } = null;

        private Vector3 sourcePos;
        private Vector3 targetPos;

        public int MinDamage { get; set; } = -1;
        public int MaxDamage { get; set; } = -1;
        public DamageType DamageType { get; set; } = DamageType.None;
        public int Accuracy { get; set; } = -1;
        public bool Pierces { get; set; } = false;

        // What happens when the projectile lands?
        public Command OnLandAction;

        private void Start()
        {
            sourcePos = Helpers.V2IToV3(Source.Position);
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
            // Move to target
            while (transform.position != targetPos)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, targetPos, .6f);

                if (Spins)
                    transform.Rotate(0, 0, 8, Space.Self);

                yield return new WaitForSeconds(.01f);
            }

            // Proj has landed at target
            if (TargetCell.Actor != null && OnLandAction == null)
            {
                if (TargetCell.Actor.RollToHit(Accuracy))
                {
                    Hit hit = new Hit(MinDamage, MaxDamage, DamageType);
                    GameLog.Send($"The {ProjName}" +
                    $" {(Pierces ? "punches through" : "hits")} " +
                    $"{Strings.GetSubject(TargetCell.Actor, false)}, " +
                    $"dealing {hit.Damage} damage!");
                    TargetCell.Actor.TakeHit(hit, Source);
                }
            }

            if (Item != null && !Returns)
                TargetCell.Items.Add(Item);

            OnLandAction?.DoAction();

            // Return to sender if set to do so
            if (Returns)
            {
                while (transform.position != sourcePos)
                {
                    transform.position =
                        Vector3.MoveTowards(transform.position, sourcePos, .6f);

                    if (Spins)
                        transform.Rotate(0, 0, 8, Space.Self);

                    yield return new WaitForSeconds(.01f);
                }
                Source.AddItem(Item);
            }

            // Done
            Game.instance.Unlock();
            Destroy(gameObject);
        }
    }
}

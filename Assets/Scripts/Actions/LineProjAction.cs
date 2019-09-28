// LineProjAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actions
{
    public enum ProjBehaviour
    {
        Fly,
        Instant
    }

    /// <summary>
    /// Prepare and fire a projectile in a line.
    /// </summary>
    [Serializable]
    public sealed class LineProjAction : BaseAction
    {
        [SerializeField] private GameObject fxPrefab;
        [SerializeField] private ProjBehaviour projBehaviour;
        [SerializeField] private bool spins;
        private BaseAction OnLandAction;

        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;
        [SerializeField] private int accuracy = -1;
        [SerializeField] private bool pierces = false; // Pierces actors

        [NonSerialized] private List<Cell> line;

        public LineProjAction(Actor actor, GameObject fxPrefab,
            ProjBehaviour projBehaviour)
            : base(actor)
        {
            this.fxPrefab = fxPrefab;
            this.projBehaviour = projBehaviour;
        }

        public LineProjAction(Actor actor, GameObject fxPrefab,
            ProjBehaviour projBehaviour, BaseAction onLand)
            : base(actor)
        {
            this.fxPrefab = fxPrefab;
            this.projBehaviour = projBehaviour;
            OnLandAction = onLand;
        }

        /// <summary>
        /// Request a line, and fire a projectile.
        /// </summary>
        public override int DoAction()
        {
            if (line != null)
            {
                FireProjectile();
                return -1;
            }

            if (Actor is Player)
                ((Player)Actor).Input.StartCoroutine(
                    ((Player)Actor).Input.LineTarget(FireProjectile));
            else
                throw new NotImplementedException("An NPC should not be able to do this");

            return -1;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            if (line != null)
            {
                FireProjectile();
                this.onConfirm = onConfirm;
                return -1;
            }

            if (Actor is Player)
                ((Player)Actor).Input.StartCoroutine(
                    ((Player)Actor).Input.LineTarget(FireProjectile));
            else
                throw new NotImplementedException("An NPC should not be able to do this");

            this.onConfirm = onConfirm;
            
            return -1;
        }

        public void SetValues(int minDamage, int maxDamage, int accuracy,
            bool pierces)
        {
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
            this.accuracy = accuracy;
            this.pierces = pierces;
        }

        public void SetValues(Ammo ammo)
        {
            minDamage = ammo.MinDamage;
            maxDamage = ammo.MaxDamage;
            accuracy = ammo.Accuracy;
            pierces = ammo.Pierces;
        }

        public void SetSpins(bool spins)
        {
            this.spins = spins;
        }

        public void SetLine(List<Cell> line) => this.line = new List<Cell>(line);

        public void FireProjectile()
        {
            if (Actor is Player)
                line = ((Player)Actor).Input.TargetLine;

            Cell startCell = Actor.Cell;
            Cell endCell = null;

            // See if anything stops the projectile
            foreach (Cell cell in line)
            {
                if (cell.Actor != null && cell.Actor != Actor)
                {
                    if (projBehaviour == ProjBehaviour.Instant)
                        HitActor(cell);

                    if (!pierces)
                    {
                        endCell = cell;
                        break;
                    }
                }
                // Stopped by terrain or features
                if (cell.Blocked)
                {
                    endCell = cell;
                    break;
                }
            }

            if (endCell == null)
            {
                // End of target line reached
                endCell = line[line.Count - 1];
            }

            switch (projBehaviour)
            {
                case ProjBehaviour.Fly:
                    {
                        Vector3 startPoint = Helpers.V2IToV3(startCell.Position);

                        GameObject projObj = UnityEngine.Object.Instantiate(
                            fxPrefab, startPoint, new Quaternion())
                            as GameObject;
                        FlyingProjectile proj = projObj.GetComponent<FlyingProjectile>();
                        proj.TargetCell = endCell;
                        proj.OnLandAction = OnLandAction;
                        proj.Spins = spins;
                    }
                    break;
                case ProjBehaviour.Instant:
                    {
                        Vector3 startPoint = Helpers.V2IToV3(startCell.Position);
                        Vector3 endPoint = Helpers.V2IToV3(endCell.Position);

                        Vector3 midPoint = (startPoint + endPoint) / 2;
                        Quaternion rotation = new Quaternion();
                        Vector3 projDirection = (startPoint - endPoint).normalized;
                        rotation = Quaternion.FromToRotation(Vector3.right, projDirection);

                        float distance = Vector3.Distance(startPoint, endPoint);

                        GameObject projObj = UnityEngine.Object.Instantiate(
                            fxPrefab, midPoint, rotation) as GameObject;
                        Vector3 scale = projObj.transform.localScale;
                        scale.x *= distance;
                        projObj.transform.localScale = scale;

                        UnityEngine.Object.Destroy(projObj, 10);

                        break;
                    }
                default:
                    throw new Exception("No projectile behaviour given.");
            }
            line = null;
            onConfirm?.Invoke();
        }

        private void HitActor(Cell cell)
        {
            int hitRoll = RandomUtils.RangeInclusive(0, 100);
            if (hitRoll < accuracy)
            {
                Hit hit = new Hit(minDamage, maxDamage);
                GameLog.Send($"The magic bullet punches through " +
                    $"{Strings.GetSubject(cell.Actor, false)}, " +
                    $"dealing {hit.Damage} damage!");
                cell.Actor.TakeHit(hit, Actor);
            }
            else
                GameLog.Send($"The magic bullet misses " +
                    $"{Strings.GetSubject(cell.Actor, false)}.");
        }

        public override string ToString()
            => $"{Actor.ActorName} is firing a {fxPrefab.name}.";
    }
}

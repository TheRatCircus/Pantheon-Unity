// LineProjAction.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;

namespace Pantheon.Actions
{
    /// <summary>
    /// Prepare and fire a projectile in a line.
    /// </summary>
    [System.Serializable]
    public class LineProjAction : BaseAction
    {
        [SerializeField] private GameObject fxPrefab;

        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;
        [SerializeField] private int accuracy = -1;
        [SerializeField] private bool pierces = false; // Pierces actors

        private List<Cell> line;

        public LineProjAction(Actor actor, GameObject fxPrefab) : base(actor)
            => this.fxPrefab = fxPrefab;

        /// <summary>
        /// Request a line, and fire a projectile by callback.
        /// </summary>
        public override int DoAction()
        {
            if (Actor is Player)
                ((Player)Actor).Input.StartCoroutine(
                    ((Player)Actor).Input.LineTarget(FireProjectile));
            else
                throw new System.NotImplementedException("An NPC should not be able to do this");

            return -1;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            if (Actor is Player)
                ((Player)Actor).Input.StartCoroutine(
                    ((Player)Actor).Input.LineTarget(FireProjectile));
            else
                throw new System.NotImplementedException("An NPC should not be able to do this");

            this.onConfirm = onConfirm;

            return -1;
        }

        public void FireProjectile()
        {
            if (Actor is Player)
                line = ((Player)Actor).Input.TargetLine;

            Cell startCell = Actor.Cell;
            Cell endCell;

            foreach (Cell cell in line)
            {
                if (cell.Actor != null && cell.Actor != Actor)
                {
                    int hitRoll = Random.Range(0, 101);
                    if (hitRoll > accuracy)
                        continue;

                    Hit hit = new Hit(minDamage, maxDamage);
                    GameLog.Send($"The magic bullet punches through " +
                        $"{Strings.GetSubject(cell.Actor, false)}, " +
                        $"dealing {hit.Damage} damage!");
                    cell.Actor.TakeHit(hit);
                    if (!pierces)
                    {
                        endCell = cell;
                        break;
                    }
                }
                // Stopped by terrain or features
                if (cell.Blocked || (cell.Feature != null && cell.Feature.Blocked))
                {
                    endCell = cell;
                    break;
                }
            }

            // End of target line reached
            endCell = line[line.Count - 1];

            Vector3 startPoint = Helpers.V2IToV3(startCell.Position);
            Vector3 endPoint = Helpers.V2IToV3(endCell.Position);

            Vector3 midPoint = (startPoint + endPoint) / 2;
            Quaternion rotation = new Quaternion();
            Vector3 projDirection = (startPoint - endPoint).normalized;
            rotation = Quaternion.FromToRotation(Vector3.right, projDirection);

            Object.Destroy(
                Object.Instantiate(
                    fxPrefab, midPoint, rotation) as GameObject, 10);

            onConfirm?.Invoke();
        }

        public override string ToString()
            => $"{Actor.ActorName} is firing a {fxPrefab.name}.";
    }
}

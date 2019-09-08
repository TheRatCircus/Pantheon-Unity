// LineProjAction.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;

namespace Pantheon.Actions
{
    /// <summary>
    /// Prepare and fire a projectile in a line.
    /// </summary>
    [System.Serializable]
    public class LineProjAction : BaseAction
    {
        [SerializeField] private GameObject projPrefab;

        List<Cell> line;

        public LineProjAction(Actor actor, GameObject projPrefab) : base(actor)
        {
            this.projPrefab = projPrefab;
        }

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

            Spawn.MakeLineProjectile(projPrefab, line);
            onConfirm?.Invoke();
        }
    }
}

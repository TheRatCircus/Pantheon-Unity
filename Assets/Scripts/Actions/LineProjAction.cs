// Fire a projectile in a line

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;

namespace Pantheon.Actions
{
    [System.Serializable]
    public class LineProjAction : BaseAction
    {
        public GameObject projPrefab;

        List<Cell> line;

        // Constructor
        public LineProjAction(Actor actor, GameObject projPrefab) : base(actor)
        {
            this.projPrefab = projPrefab;
        }

        // Request a line, and fire a projectile by callback
        public override int DoAction()
        {
            if (Actor is Player)
                ((Player)Actor)._input.StartCoroutine(
                    ((Player)Actor)._input.LineTarget(FireProjectile));
            else
                throw new System.NotImplementedException("An NPC should not be able to do this");

            return -1;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            if (Actor is Player)
                ((Player)Actor)._input.StartCoroutine(
                    ((Player)Actor)._input.LineTarget(FireProjectile));
            else
                throw new System.NotImplementedException("An NPC should not be able to do this");

            this.onConfirm = onConfirm;

            return -1;
        }

        // Fire the projectile
        public void FireProjectile()
        {
            if (Actor is Player)
                line = ((Player)Actor)._input.TargetLine;

            Spawn.MakeLineProjectile(projPrefab, line);
            onConfirm?.Invoke();
        }
    }
}

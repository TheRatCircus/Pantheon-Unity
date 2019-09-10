// NPC.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using Pantheon.Actions;
using Pantheon.Utils;

namespace Pantheon.Actors
{
    public class NPC : Actor
    {
        // Requisite objects
        private SpriteRenderer spriteRenderer;

        [ReadOnly] private Actor target;

        // Awake is called when the first script instance is being loaded
        protected override void Awake() => base.Awake();

        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = cell.Visible;
        }

        // Every time something happens, this NPC must refresh its visibility
        public void UpdateVisibility()
            => spriteRenderer.enabled = cell.Visible;

        // Evaluate the situation and act
        public override int Act()
        {
            // Detect player if coming into player's view
            if (cell.Visible && target == null)
            {
                target = Game.GetPlayer();
                GameLog.Send($"{Strings.GetSubject(this, true)} notices you!", MessageColour.Red);
            }

            // Engage in combat
            if (target != null)
                if (!level.AdjacentTo(cell, target.Cell))
                    PathMoveToTarget();
                else
                    NextAction = new MeleeAction(this, target);
            else
                NextAction = new WaitAction(this);

            BaseAction ret = NextAction;
            // Clear action buffer
            NextAction = null;
            return ret.DoAction();
        }

        // Make a single move along a path towards a target
        void PathMoveToTarget()
        {
            List<Cell> path = level.Pathfinder.GetCellPath(Position, target.Position);
            if (path.Count > 0)
                NextAction = new MoveAction(this, MoveSpeed, path[0]);
        }

        // Handle NPC death
        protected override void OnDeath()
        {
            base.OnDeath();
            level.NPCs.Remove(this);
        }
    }
}

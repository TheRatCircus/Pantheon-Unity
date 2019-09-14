// NPC.cs
// Jerome Martina

#define DEBUG_NPC
#undef DEBUG_NPC

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
        [SerializeField] [ReadOnly] private Actor target;

        // Events
        public event System.Action<bool> OnVisibilityChangeEvent;

        // Awake is called when the first script instance is being loaded
        protected override void Awake() => base.Awake();

        private void OnEnable()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = cell.Visible;
        }

        // Every time something happens, this NPC must refresh its visibility
        public void UpdateVisibility()
        {
            spriteRenderer.enabled = cell.Visible;
            OnVisibilityChangeEvent?.Invoke(cell.Visible);
        }

        // Evaluate the situation and act
        public override int Act()
        {
            // Detect player if coming into player's view
            if (cell.Visible && target == null)
            {
                target = Game.GetPlayer();
                GameLog.Send($"{Strings.GetSubject(this, true)} notices you!",
                    Strings.TextColour.Red);
            }

            // Engage in combat
            if (target != null)
                if (!level.AdjacentTo(cell, target.Cell))
                    PathMoveToTarget();
                else
                    NextAction = new MeleeAction(this, target);
            else
                NextAction = new WaitAction(this);

            LogNPCAction();

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
        public override void OnDeath()
        {
            base.OnDeath();
            level.NPCs.Remove(this);
        }

        [System.Diagnostics.Conditional("DEBUG_NPC")]
        private void LogNPCAction()
        {
            UnityEngine.Debug.Log($"{actorName} next action: {nextAction}");
        }
    }
}

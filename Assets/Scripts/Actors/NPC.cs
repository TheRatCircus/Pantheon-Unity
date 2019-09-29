// NPC.cs
// Jerome Martina

#define DEBUG_NPC
#undef DEBUG_NPC

using Pantheon.Actions;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actors
{
    public enum Intelligence
    {
        None,
        Animal,
        Human
    }

    public class NPC : Actor
    {
        [SerializeField] private Intelligence intelligence;
        [SerializeField] private bool alwaysHostileToPlayer = false;

        [SerializeField] [ReadOnly] private Actor target;
        public bool AwareOfPlayer { get; private set; } = false;
        
        // Properties
        public Intelligence Intelligence
        {
            get => intelligence;
            private set => intelligence = value;
        }
        public bool AlwaysHostileToPlayer
        {
            get => alwaysHostileToPlayer;
            private set => alwaysHostileToPlayer = value;
        }

        // Events
        public event System.Action<bool> OnVisibilityChangeEvent;

        // Awake is called when the first script instance is being loaded
        protected override void Awake()
        {
            base.Awake();
            inventory = new Inventory(10);
        }

        private void OnEnable()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = cell.Visible;
            if (intelligence == Intelligence.Animal)
                Faction = Game.instance.Nature;
        }

        /// <summary>
        /// Call if referencing contained objects before Awake or Enable.
        /// </summary>
        public void Initialize()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            inventory = new Inventory(10);
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
            // Random energy
            int r = RandomUtils.RangeInclusive(0, 20);
            if (r >= 18)
                energy += speed / 10;
            else if (r <= 2)
                energy -= speed / 10;

            // Detect player if coming into player's view
            if (cell.Visible && !AwareOfPlayer)
                DetectPlayer();

            // Engage in combat
            if (target != null)
                if (!level.AdjacentTo(cell, target.Cell))
                    PathMoveToTarget();
                else
                    NextAction = new MeleeAction(this, target.Cell);
            else
                NextAction = new WaitAction(this);

            LogNPCAction();

            BaseAction ret = NextAction;
            // Clear action buffer
            NextAction = null;
            return ret.DoAction();
        }

        public override void TakeHit(Hit hit, Actor source)
        {
            base.TakeHit(hit, source);
            if (source is Player && !(target is Player))
            {
                AlwaysHostileToPlayer = true;
                target = Game.GetPlayer();
                GameLog.Send($"{Strings.GetSubject(this, true)}" +
                    $" fixes its gaze upon you!",
                    Strings.TextColour.Orange);
            }
            else
                target = source;
        }

        private void DetectPlayer()
        {
            if (IsHostileTo(Game.GetPlayer()))
            {
                target = Game.GetPlayer();
                GameLog.Send($"{Strings.GetSubject(this, true)}" +
                    $" fixes its gaze upon you!",
                    Strings.TextColour.Orange);
            }
            else
            {
                GameLog.Send($"{Strings.GetSubject(this, true)} notices you.",
                    Strings.TextColour.White);
            }
            AwareOfPlayer = true;
        }

        // Make a single move along a path towards a target
        private void PathMoveToTarget()
        {
            List<Cell> path = level.Pathfinder.GetCellPath(Position, target.Position);
            if (path.Count > 0)
                NextAction = new MoveAction(this, MoveTime, path[0]);
        }

        public string LongDescription()
        {
            string ret = $"{ActorName} ";

            if (inventory.Wielded.Count > 0)
            {
                ret += "wielding ";
                foreach (Item item in inventory.Wielded)
                    ret += $" a {item.DisplayName};";
            }
            
            return ret;
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

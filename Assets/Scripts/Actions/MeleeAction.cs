// MeleeAction.cs
// Jerome Martina

#define DEBUG_MELEE
#undef DEBUG_MELEE

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.Utils;

namespace Pantheon.Actions
{
    /// <summary>
    /// Try to land a melee hit on an actor.
    /// </summary>
    public class MeleeAction : BaseAction
    {
        private Actor target;

        public MeleeAction(Actor actor, Actor target)
            : base(actor)
            => this.target = target;

        /// <summary>
        /// Attack an actor; if successful, cause the target to take a hit.
        /// </summary>
        /// <returns>The time taken by the attack.</returns>
        public override int DoAction()
        {
            int actionTime = -1;

            List<Melee> attacks = Actor.GetMelees();
            if (attacks == null)
            {
                Debug.LogWarning("An actor has no melee attacks.");
                return Game.TurnTime;
            }
            
            // Action takes the time of the slowest, so get that first
            foreach (Melee attack in attacks)
                if (attack.AttackTime > actionTime)
                    actionTime = attack.AttackTime;

            if (actionTime < 0)
                throw new System.Exception("A MeleeAction has no attack time.");

            /*  
             *  Now iterate directly through parts in order to send relevant
             *  data to string processing for game log.
             *  
             *  e.g. if attack takes 200 and another takes 50, one attack of
             *  200 and four of 50 are carried out in the action
             */
            foreach (BodyPart part in Actor.Parts)
            {
                if (target.IsDead())
                    break;

                bool weapon = false; // Whether or not an item is involved
                Melee attack;
                int swings;

                if (part.Item != null)
                {
                    weapon = true;
                    attack = part.Item.Melee;
                    swings = actionTime / attack.AttackTime;
                }
                else if (part.CanMelee)
                {
                    attack = part.Melee;
                    swings = actionTime / attack.AttackTime;
                }
                else { continue; }

                for (int i = 0; i < swings; i++)
                {
                    if (target.IsDead())
                        break;

                    string attackMsg = "";

                    // The attacker, e.g. "you" or "the enemy"
                    attackMsg += $"{Strings.GetSubject(Actor, true)} ";

                    // Did the attack hit?
                    bool hitLanded = HitLanded(attack);

                    if (hitLanded)
                    {
                        // Hit string, e.g. "slice" or "bites"
                        if (weapon)
                            attackMsg += $"{Strings.WeaponHitString(Actor, part.Item)} ";
                        else
                            attackMsg += $"{Strings.PartHitString(Actor, part)} ";
                    }
                    else
                        attackMsg += $"{(Actor is Player ? "miss" : "misses")} ";

                    // The defender
                    attackMsg += $"{Strings.GetSubject(target, false)}";

                    if (!hitLanded)
                    {
                        attackMsg += ".";
                        GameLog.Send(attackMsg, Strings.TextColour.Grey);
                    }
                    else
                    {
                        Hit hit = new Hit(attack.MinDamage, attack.MaxDamage);
                        attackMsg += $" for {hit.Damage} damage!";
                        GameLog.Send(attackMsg, Strings.TextColour.Grey);
                        target.TakeHit(hit);
                    }
#if DEBUG_MELEE
                    string actorName = Actor.ActorName;
                    string targetName = target.ActorName;
                    string attackSourceName = weapon ? part.Item.DisplayName : part.Name;
                    Debug.Log($"{actorName} attacked {targetName}({target.Health} HP) with {attackSourceName}, hit landed: {hitLanded}");

                    if (target.IsDead())
                        Debug.Log($"{actorName} has killed {targetName}");
#endif
                }
            }
            return actionTime;
        }

        public bool HitLanded(Melee attack)
        {
            int hitRoll = Random.Range(0, 101);
            return hitRoll <= attack.Accuracy;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}

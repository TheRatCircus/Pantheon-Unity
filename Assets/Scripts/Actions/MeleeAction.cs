// MeleeAction.cs
// Jerome Martina

#define DEBUG_MELEE
#undef DEBUG_MELEE

using Pantheon.Actors;
using Pantheon.Components;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actions
{
    /// <summary>
    /// Try to land a melee hit on an actor.
    /// </summary>
    public sealed class MeleeAction : BaseAction
    {
        private Cell target;

        public MeleeAction(Actor actor, Cell target)
            : base(actor)
            => this.target = target;

        /// <summary>
        /// Attack an actor; if successful, cause the target to take a hit.
        /// </summary>
        /// <returns>The time taken by the attack.</returns>
        public override int DoAction()
        {
            Actor enemy;
            if (target.Actor != null)
                enemy = target.Actor;
            else
                enemy = null;

            int actionTime = -1;

            List<Melee> attacks = Actor.Body.GetMelees();
            if (attacks == null)
            {
                UnityEngine.Debug.LogWarning("An actor has no melee attacks.");
                return Game.TurnTime;
            }
            
            // Action takes the time of the slowest, so get that first
            foreach (Melee attack in attacks)
                if (attack.AttackTime > actionTime)
                    actionTime = attack.AttackTime;

            if (actionTime < 0)
                throw new System.Exception("A MeleeAction has no attack time.");

            // Track already-used weapons to prevent re-use by a different limb
            HashSet<Item> weaponStatuses = new HashSet<Item>();

            /*  
             *  Now iterate directly through parts in order to send relevant
             *  data to string processing for game log.
             *  
             *  e.g. if attack takes 200 and another takes 50, one attack of
             *  200 and four of 50 are carried out in the action
             */
            foreach (BodyPart part in Actor.Body.Parts)
            {
                if (enemy != null && enemy.IsDead())
                    break;

                bool weapon = false; // Whether or not an item is involved
                Melee attack;
                int swings;

                if (part.Item != null && !weaponStatuses.Contains(part.Item))
                {
                    weapon = true;
                    attack = part.Item.Melee;
                    swings = actionTime / attack.AttackTime;
                }
                else if (part.CanMelee && part.Dexterous)
                {
                    attack = part.Melee;
                    swings = actionTime / attack.AttackTime;
                }
                else { continue; }

                for (int i = 0; i < swings; i++)
                {
                    if (enemy != null && enemy.IsDead())
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
                    if (enemy != null)
                        attackMsg += $"{Strings.GetSubject(enemy, false)}";
                    else
                    {
                        if (target.Feature != null)
                            attackMsg += $"the {target.Feature.DisplayName}";
                        else if (target.Blocked)
                            attackMsg += $"the {target.TerrainData.DisplayName}";
                        else
                            attackMsg += $"the air";
                    }

                    if (!hitLanded)
                    {
                        attackMsg += ".";
                        GameLog.Send(attackMsg, Strings.TextColour.Grey);
                    }
                    else
                    {
                        Hit hit = new Hit(attack.MinDamage, attack.MaxDamage);

                        if (enemy != null)
                        {
                            attackMsg += $" for {hit.Damage} damage!";
                            GameLog.Send(attackMsg, Strings.TextColour.White);
                            enemy.TakeHit(hit, Actor);
                        }
                        else
                        {
                            attackMsg += ".";
                            GameLog.Send(attackMsg, Strings.TextColour.Grey);
                        }
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
                // Prevent a weapon from being swung twice by two appendages
                if (weapon)
                    weaponStatuses.Add(part.Item);
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

        public override string ToString()
            => $"{Actor.ActorName} is attacking {target} in melee.";
    }
}

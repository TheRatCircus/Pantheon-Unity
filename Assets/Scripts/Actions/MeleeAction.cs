// MeleeAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;

namespace Pantheon.Actions
{
    /// <summary>
    /// Try to land a melee hit on an actor.
    /// </summary>
    public class MeleeAction : BaseAction
    {
        private Actor target;
        private readonly int attackTime;

        public MeleeAction(Actor actor, int attackTime, Actor target)
            : base(actor)
        {
            this.attackTime = attackTime;
            this.target = target;
        }

        /// <summary>
        /// Attack an actor; if the hit lands, cause the target to take a hit.
        /// </summary>
        /// <returns>The time taken by the attack.</returns>
        public override int DoAction()
        {
            // Did the attack hit?
            bool hitLanded = UnityEngine.Random.Range(0, 101) <= Actor.Accuracy;

            if (hitLanded)
            {
                Hit hit = new Hit(Actor.MinDamage, Actor.MaxDamage);
                target.TakeHit(hit);
                GameLog.Send(GameLog.GetAttackString(Actor, target, hit), MessageColour.White);
            }
            else GameLog.Send(GameLog.GetAttackString(Actor, target, null), MessageColour.White);

            return attackTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}

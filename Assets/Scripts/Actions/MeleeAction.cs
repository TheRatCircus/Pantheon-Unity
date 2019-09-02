// Attack an actor in melee combat
namespace Pantheon.Actions
{
    public class MeleeAction : BaseAction
    {
        public Actor target;
        public int attackTime;

        // Constructor
        public MeleeAction(Actor actor, int attackTime, Actor target)
            : base(actor)
        {
            this.attackTime = attackTime;
            this.target = target;
        }

        // Attempt to hit the target actor with a melee attack
        public override int DoAction()
        {
            // Did the attack hit?
            bool hitLanded = UnityEngine.Random.Range(0, 101) <= Actor.accuracy;

            if (hitLanded)
            {
                Hit hit = new Hit(Actor.minDamage, Actor.maxDamage);
                target.TakeHit(hit);
                GameLog.Send(GameLog.GetAttackString(Actor, target, hit), MessageColour.White);
            }
            else GameLog.Send(GameLog.GetAttackString(Actor, target, null), MessageColour.White);

            return attackTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            throw new System.NotImplementedException();
        }
    }
}

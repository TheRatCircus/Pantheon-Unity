// Action which heals an actor

using Pantheon.Actors;

namespace Pantheon.Actions
{
    [System.Serializable]
    public class HealAction : BaseAction
    {
        public int HealAmount;
        public float HealPercent;

        // Constructor
        public HealAction(Actor actor, int healAmount, float healPercent)
            : base(actor)
        {
            HealAmount = healAmount;
            HealPercent = healPercent;
        }

        // Attempt to heal the actor
        public override int DoAction()
        {
            Actor.Heal(HealAmount);
            Actor.Heal((int)(Actor.MaxHealth * HealPercent));
            return -1;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            Actor.Heal(HealAmount);
            Actor.Heal((int)(Actor.MaxHealth * HealPercent));
            onConfirm?.Invoke();
            return -1;
        }
    }
}

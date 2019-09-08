// Action which heals an actor

using UnityEngine;
using Pantheon.Actors;

namespace Pantheon.Actions
{
    [System.Serializable]
    public class HealAction : BaseAction
    {
        [SerializeField] private int healAmount;
        [SerializeField] private float healPercent;

        // Constructor
        public HealAction(Actor actor, int healAmount, float healPercent)
            : base(actor)
        {
            this.healAmount = healAmount;
            this.healPercent = healPercent;
        }

        // Attempt to heal the actor
        public override int DoAction()
        {
            Actor.Heal(healAmount);
            Actor.Heal((int)(Actor.MaxHealth * healPercent));
            return -1;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            Actor.Heal(healAmount);
            Actor.Heal((int)(Actor.MaxHealth * healPercent));
            onConfirm?.Invoke();
            return -1;
        }
    }
}

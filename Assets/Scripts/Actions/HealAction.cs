// HealAction.cs
// Jerome Martina

using Pantheon.Actors;
using UnityEngine;

namespace Pantheon.Actions
{
    /// <summary>
    /// Heal an actor.
    /// </summary>
    [System.Serializable]
    public sealed class HealAction : BaseAction
    {
        [SerializeField] private int healAmount;
        [SerializeField] private float healPercent;

        public HealAction(Actor actor, int healAmount, float healPercent)
            : base(actor)
        {
            this.healAmount = healAmount;
            this.healPercent = healPercent;
        }

        /// <summary>
        /// Attempt to heal the actor.
        /// </summary>
        /// <returns>Negative action cost.</returns>
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

        public override string ToString()
            => $"{Actor.ActorName} is being healed for " +
            $"{healAmount}/{healPercent * 100}%.";
    }
}

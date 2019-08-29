// 
namespace Pantheon.Actions
{
    public class HealAction : BaseAction
    {
        public int HealAmount;
        public float HealPercent;

        public HealAction(Actor actor, int healAmount, float healPercent)
            : base(actor)
        {
            ActionCost = -1; // Real cost is from casting spell, using item, etc.
            HealAmount = healAmount;
            HealPercent = healPercent;
        }

        public override int DoAction()
        {
            actor.Health += HealAmount;
            actor.Health += (int)(actor.MaxHealth * HealPercent);
            return ActionCost;
        }
    }
}

// 
namespace Pantheon.Actions
{
    [System.Serializable]
    public class ItemUseAction : BaseAction
    {
        public Item item;
        public BaseAction onUse;

        public ItemUseAction(Actor actor, Item item) : base(actor)
        {
            this.item = item;
            ActionCost = Game.TurnTime;
        }

        public ItemUseAction(Actor actor, Item item, BaseAction onUse) : base(actor)
        {
            this.item = item;
            this.onUse = onUse;
            ActionCost = Game.TurnTime;
        }

        public override int DoAction()
        {
            onUse.DoAction();
            actor.RemoveItem(item);
            return ActionCost;
        }
    }
}

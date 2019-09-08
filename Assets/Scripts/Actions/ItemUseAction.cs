// Use an item

using Pantheon.Actors;

namespace Pantheon.Actions
{
    public class ItemUseAction : BaseAction
    {
        public Item Item;
        public BaseAction OnUse;

        public int UseTime;

        // Constructor
        public ItemUseAction(Actor actor, Item item, BaseAction onUse) : base(actor)
        {
            Item = item;
            OnUse = onUse;
            UseTime = Core.Game.TurnTime;

            onUse.DoAction(AssignAction);
        }
        
        // Finish using the item
        public override int DoAction()
        {
            Actor.RemoveItem(Item);
            return UseTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            throw new System.NotImplementedException();
        }
    }
}

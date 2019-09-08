// Use an item

using Pantheon.Actors;

namespace Pantheon.Actions
{
    public class ItemUseAction : BaseAction
    {
        private readonly Item item;
        private readonly BaseAction onUse;

        private readonly int useTime;

        // Constructor
        public ItemUseAction(Actor actor, Item item, BaseAction onUse) : base(actor)
        {
            this.item = item;
            this.onUse = onUse;
            useTime = Core.Game.TurnTime;

            onUse.DoAction(AssignAction);
        }
        
        // Finish using the item
        public override int DoAction()
        {
            Actor.RemoveItem(item);
            return useTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            throw new System.NotImplementedException();
        }
    }
}

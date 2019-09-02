// Drop an item from an inventory
namespace Pantheon.Actions
{
    public class DropAction : BaseAction
    {
        public Item Item;

        // Constructor
        public DropAction(Actor actor, Item item) : base(actor)
        {
            Item = item;
        }

        // Drop the item
        public override int DoAction()
        {
            if (Item == null)
                throw new System.Exception("Attempt to drop a null item");

            if (!Actor.Inventory.Contains(Item))
                throw new System.Exception("Attempt to drop item from outside inventory");

            Actor.RemoveItem(Item);
            Actor.Cell.Items.Add(Item);

            if (Actor is Player)
            {
                GameLog.Send($"You drop the {Item.DisplayName}.", MessageColour.Grey);
                ((Player)Actor).RaiseInventoryChangeEvent();
            }

            return Game.TurnTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
        {
            throw new System.NotImplementedException();
        }
    }
}
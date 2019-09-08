// Drop an item from an inventory

using Pantheon.Actors;

namespace Pantheon.Actions
{
    public class DropAction : BaseAction
    {
        private Item item;

        public DropAction(Actor actor, Item item)
            : base(actor)
            => this.item = item;

        // Drop the item
        public override int DoAction()
        {
            if (item == null)
                throw new System.Exception("Attempt to drop a null item");

            if (!Actor.Inventory.Contains(item))
                throw new System.Exception("Attempt to drop item from outside inventory");

            Actor.RemoveItem(item);
            Actor.Cell.Items.Add(item);

            if (Actor is Player)
            {
                Core.GameLog.Send($"You drop the {item.DisplayName}.", MessageColour.Grey);
                ((Player)Actor).RaiseInventoryChangeEvent();
            }

            return Core.Game.TurnTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}
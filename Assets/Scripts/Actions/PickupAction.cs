// Pick up an item and add it to inventory

using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;

namespace Pantheon.Actions
{
    public class PickupAction : BaseAction
    {
        public Cell Cell;
        public Item Item;

        // Constructor
        public PickupAction(Actor actor, Cell cell)
            : base(actor)
            => Cell = cell;

        // Try to pick up the first item in the cell's stack of items
        public override int DoAction()
        {
            if (Cell.Items.Count == 0)
            {
                GameLog.Send("There is nothing here to pick up.", MessageColour.Grey);
                return -1;
            }
            else
            {
                Item = Cell.Items[0];
                Cell.Items.RemoveAt(0);
                Actor.Inventory.Add(Item);
                Item.Owner = Actor;

                if (Actor is Player)
                {
                    ((Player)Actor).RaiseInventoryChangeEvent();
                    GameLog.Send($"You pick up a {Item.DisplayName}.", MessageColour.White);
                }
                
                return Game.TurnTime;
            }
        }

        // DoAction with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}

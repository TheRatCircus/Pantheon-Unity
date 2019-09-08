// PickupAction.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;

namespace Pantheon.Actions
{
    /// <summary>
    /// An actor tries to pick an item up off the ground.
    /// </summary>
    public class PickupAction : BaseAction
    {
        private Cell cell;
        private Item item;

        public PickupAction(Actor actor, Cell cell)
            : base(actor)
            => this.cell = cell;

        /// <summary>
        /// Try to pick up the first item in the cell's stack of items.
        /// </summary>
        public override int DoAction()
        {
            if (cell.Items.Count == 0)
            {
                GameLog.Send("There is nothing here to pick up.", MessageColour.Grey);
                return -1;
            }
            else
            {
                item = cell.Items[0];
                cell.Items.RemoveAt(0);
                Actor.Inventory.Add(item);
                item.Owner = Actor;

                if (Actor is Player)
                {
                    ((Player)Actor).RaiseInventoryChangeEvent();
                    GameLog.Send($"You pick up a {item.DisplayName}.", MessageColour.White);
                }
                
                return Game.TurnTime;
            }
        }

        // DoAction with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}

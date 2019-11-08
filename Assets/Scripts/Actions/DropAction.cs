// DropAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Utils;

namespace Pantheon.Actions
{
    /// <summary>
    /// Drop an item from an inventory.
    /// </summary>
    public sealed class DropAction : Command
    {
        private Item item;

        public DropAction(Actor actor, Item item)
            : base(actor)
            => this.item = item;

        /// <summary>
        /// Drop the item to the cell below the actor.
        /// </summary>
        /// <returns>Time taken to drop the item.</returns>
        public override int DoAction()
        {
            if (item == null)
                throw new System.Exception("Attempt to drop a null item");

            if (!Actor.Inventory.All.Contains(item))
                throw new System.Exception("Attempt to drop item from outside inventory");

            Actor.RemoveItem(item);
            Actor.Cell.Items.Add(item);

            if (Actor is Player)
            {
                Core.GameLog.Send($"You drop the {item.DisplayName}.",
                    Strings.TextColour.Grey);
            }

            return Core.Game.TurnTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is dropping {item.DisplayName}.";
    }
}
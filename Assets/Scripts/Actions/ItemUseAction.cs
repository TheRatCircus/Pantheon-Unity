// ItemUseAction.cs
// Jerome Martina

using Pantheon.Actors;

namespace Pantheon.Actions
{
    /// <summary>
    /// Attempt to use an item.
    /// </summary>
    public sealed class ItemUseAction : Command
    {
        private readonly Item item;
        private readonly Command onUse;

        private readonly int useTime;

        public ItemUseAction(Actor actor, Item item, Command onUse)
            : base(actor)
        {
            this.item = item;
            this.onUse = onUse;
            useTime = Core.Game.TurnTime;

            onUse.DoAction(AssignAction);
        }
        
        /// <summary>
        /// Finish using the item and consume it if appropriate.
        /// </summary>
        /// <returns>The item's use time.</returns>
        public override int DoAction()
        {
            Actor.RemoveItem(item);
            return useTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is using {item.DisplayName}.";
    }
}

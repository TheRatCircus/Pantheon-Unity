// WieldAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.Utils;
using System.Collections.Generic;

namespace Pantheon.Actions
{
    /// <summary>
    /// An actor attempts to wield an item in one or more prehensile appendages.
    /// </summary>
    public sealed class WieldAction : Command
    {
        private readonly Item item;
        private readonly Appendage[] appendages;

        public WieldAction(Actor actor, Item item, Appendage[] appendages)
            : base(actor)
        {
            this.item = item;
            this.appendages = appendages;
        }

        // Alternate constructor for lists
        public WieldAction(Actor actor, Item item, List<Appendage> appendages)
            : base(actor)
        {
            this.item = item;
            this.appendages = appendages.ToArray();
        }

        public override int DoAction()
        {
            Actor.Inventory.Wielded.Remove(item);
            // Unwield item from all previous appendages
            foreach (Appendage app in Actor.Body.GetPrehensiles())
            {
                if (app.Item == item)
                    app.Item = null;
            }

            foreach (Appendage app in appendages)
                app.Item = item;

            Actor.Inventory.Wielded.Add(item);
            item.OnEquip(Actor);

            item.WieldProfile = appendages;

            if (Actor is Player)
                GameLog.Send($"You wield the {item.DisplayName}.",
                    Strings.TextColour.White);

            return Game.TurnTime;
        }

        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is wielding {item.DisplayName}";
    }
}

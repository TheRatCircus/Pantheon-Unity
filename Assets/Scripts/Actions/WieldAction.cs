// WieldAction.cs
// Jerome Martina

using System.Collections.Generic;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.Utils;

namespace Pantheon.Actions
{
    /// <summary>
    /// An actor attempts to wield an item in one or more prehensile parts.
    /// </summary>
    public class WieldAction : BaseAction
    {
        private readonly Item item;
        private readonly BodyPart[] parts;

        public WieldAction(Actor actor, Item item, BodyPart[] parts)
            : base(actor)
        {
            this.item = item;
            this.parts = parts;
        }

        // Alternate constructor for lists
        public WieldAction(Actor actor, Item item, List<BodyPart> parts)
            : base(actor)
        {
            this.item = item;
            this.parts = parts.ToArray();
        }

        public override int DoAction()
        {
            // Unwield item from all previous parts
            foreach (BodyPart part in Actor.GetPrehensiles())
            {
                if (part.Item == item)
                    part.Item = null;
            }

            foreach (BodyPart part in parts)
                part.Item = item;

            item.WieldProfile = parts;

            GameLog.Send($"You wield the {item.DisplayName}.", Strings.TextColour.White);

            return Game.TurnTime;
        }

        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is wielding {item.DisplayName}";
    }
}

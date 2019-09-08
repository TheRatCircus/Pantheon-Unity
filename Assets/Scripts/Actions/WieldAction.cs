// WieldAction.cs
// Jerome Martina

using System.Collections.Generic;
using Pantheon.Core;
using Pantheon.Actors;

namespace Pantheon.Actions
{
    public class WieldAction : BaseAction
    {
        Item item;
        BodyPart[] parts;

        public WieldAction(Actor actor, Item item, BodyPart[] parts)
            : base(actor)
        {
            this.item = item;
            this.parts = parts;
        }

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

            GameLog.Send($"You wield the {item.DisplayName}.", MessageColour.White);

            return Game.TurnTime;
        }

        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}

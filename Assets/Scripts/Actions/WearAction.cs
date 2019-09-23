// WearAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using System;

namespace Pantheon.Actions
{
    public sealed class WearAction : BaseAction
    {
        private Item item;

        public WearAction(Actor actor, Item item)
            : base(actor) => this.item = item;

        public override int DoAction()
        {
            switch (item.EquipType)
            {
                case EquipType.Body:
                    Actor.Equipment.BodyWear = item;
                    break;
                default:
                    throw new Exception("Illegal equip type.");
            }

            Actor.Defenses.Recalculate(Actor);

            if (Actor is Player player)
            {
                GameLog.Send($"You put on the {item.DisplayName}.");
                player.RemoveItem(item);
            }
            else
            {
                if (!Actor.Inventory.RemoveItem(item))
                    throw new NotImplementedException
                        ("Actor wore item not in inventory.");
            }

            return Game.TurnTime;
        }

        public override int DoAction(OnConfirm onConfirm)
            => throw new NotImplementedException();
    }
}
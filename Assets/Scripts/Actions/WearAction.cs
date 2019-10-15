// WearAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Components;
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
            switch (item.GetComponent<Components.Equipment>().EquipType)
            {
                case EquipType.Head:
                    Actor.Equipment.HeadWear = item;
                    break;
                case EquipType.Body:
                    Actor.Equipment.BodyWear = item;
                    break;
                case EquipType.Shoulders:
                    Actor.Equipment.ShoulderWear = item;
                    break;
                case EquipType.Gloves:
                    Actor.Equipment.Handwear = item;
                    break;
                case EquipType.Waist:
                    Actor.Equipment.Belt = item;
                    break;
                case EquipType.Feet:
                    Actor.Equipment.Footwear = item;
                    break;
                default:
                    throw new Exception("Illegal equip type.");
            }

            item.OnEquip(Actor);

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
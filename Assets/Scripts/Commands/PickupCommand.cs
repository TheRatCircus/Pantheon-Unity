// PickupCommand.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;

namespace Pantheon.Commands
{
    public sealed class PickupCommand : ActorCommand
    {
        public PickupCommand(Entity entity) : base(entity) { }

        public override CommandResult Execute(out int cost)
        {
            bool player = Entity.PlayerControlled;

            if (!Entity.TryGetComponent(out Inventory inv))
                throw new System.Exception(
                    $"{Entity} has no inventory.");

            if (Entity.Cell.Items.Count < 1)
            {
                cost = -1;
                if (player)
                    LogLocator.Service.Send(
                        "There is nothing here to take.",
                        Color.grey);
                return CommandResult.Cancelled;
            }

            Entity item = Entity.Cell.Items[0];
            item.Cell.Items.Remove(item);
            inv.AddItem(item);
            cost = Core.TurnScheduler.TurnTime;
            if (player)
                LogLocator.Service.Send(
                    $"You pick up a {item}.",
                    Color.grey);
            return CommandResult.Succeeded;
        }
    }
}
// DropCommand.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;

namespace Pantheon.Commands
{
    /// <summary>
    /// Drop an item from one's inventory into the cell below.
    /// </summary>
    public sealed class DropCommand : ActorCommand
    {
        public DropCommand(Entity entity) : base(entity) { }

        public override CommandResult Execute(out int cost)
        {
            bool player = Entity.PlayerControlled;

            if (!Entity.TryGetComponent(out Inventory inv))
                throw new System.Exception(
                    $"{Entity} has no inventory.");

            if (inv.Items.Count < 1 && player)
            {
                LogLocator.Service.Send(
                    $"You have nothing in your inventory to drop.",
                    Color.grey);
                cost = -1;
                return CommandResult.Cancelled;
            }

            Entity item = inv.Items[0];
            inv.RemoveItem(item);
            item.Cell.Items.Add(item);
            cost = Core.TurnScheduler.TurnTime;
            if (player)
                LogLocator.Service.Send(
                    $"You drop a {item}.",
                    Color.grey);
            return CommandResult.Succeeded;
        }
    }
}

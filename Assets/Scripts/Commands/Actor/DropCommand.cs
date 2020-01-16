// DropCommand.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;

namespace Pantheon.Commands.Actor
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

            if (inv.Items.Count < 1)
            {
                if (player)
                    Locator.Log.Send(
                        $"You have nothing in your inventory to drop.",
                        Color.grey);
                else
                    UnityEngine.Debug.LogWarning(
                        $"NPC {Entity} tried to drop from an empty inventory.");

                cost = -1;
                return CommandResult.Failed;
            }

            Entity item = inv.Items[0];
            inv.RemoveItem(item);
            //item.Cell.AddEntity(item);
            cost = Core.TurnScheduler.TurnTime;

            if (player)
                Locator.Log.Send($"You drop a {item}.", Color.grey);

            return CommandResult.Succeeded;
        }
    }
}

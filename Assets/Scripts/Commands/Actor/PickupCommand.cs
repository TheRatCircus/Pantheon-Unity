// PickupCommand.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;

namespace Pantheon.Commands.Actor
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

            if (!Entity.Cell.TryGetItem(0, out Entity item))
            {
                cost = -1;

                if (player)
                    Locator.Log.Send(
                        "There is nothing here to take.",
                        Color.grey);
                else
                    UnityEngine.Debug.LogWarning(
                        $"NPC {Entity} tried to pickup from an empty cell.");

                return CommandResult.Failed;
            }

            //item.Cell.RemoveEntity(item);
            inv.AddItem(item);
            cost = Core.TurnScheduler.TurnTime;

            if (player)
                Locator.Log.Send(
                    $"You pick up {item.ToSubjectString(false)}.",
                    Color.grey);

            return CommandResult.Succeeded;
        }
    }
}
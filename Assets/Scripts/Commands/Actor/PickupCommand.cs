// PickupCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Commands.Actor
{
    using Actor = Components.Entity.Actor;

    public sealed class PickupCommand : ActorCommand
    {
        public PickupCommand(Entity entity) : base(entity)
        {
            Cost = Core.TurnScheduler.TurnTime;
        }

        public override CommandResult Execute()
        {
            bool player = Actor.PlayerControlled(Entity);

            if (!Entity.TryGetComponent(out Inventory inv))
                throw new System.Exception(
                    $"{Entity} has no inventory.");

            List<Entity> items = Level.ItemsAt(Entity.Cell.x, Entity.Cell.y);

            if (items.Count < 1)
            {
                if (player)
                    Locator.Log.Send(
                        "There is nothing here to take.",
                        Color.grey);
                else
                    UnityEngine.Debug.LogWarning(
                        $"NPC {Entity} tried to pickup from an empty cell.");

                return CommandResult.Failed;
            }

            inv.AddItem(items[0]);

            if (player)
                Locator.Log.Send(
                    $"You pick up {Strings.Subject(items[0], false)}.",
                    Color.grey);

            return CommandResult.Succeeded;
        }
    }
}
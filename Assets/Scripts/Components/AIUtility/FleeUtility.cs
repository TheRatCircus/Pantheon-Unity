// FleeUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Utils;
using System;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    public sealed class FleeUtility : AIUtility
    {
        public override int Recalculate(Entity entity, AI ai)
        {
            int health = entity.GetComponent<Health>().Current;
            double y = 1 / (1 + Math.Pow(Math.E, 4 * (health - .5f)));
            int utility = (int)Math.Floor(y);
            return utility;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            int dist = Helpers.Distance(entity.Cell, Locator.Player.Entity.Cell);
            Vector2Int cell = Floodfill.QueueFillForCell(
                entity.Level, entity.Cell,
                (Vector2Int c) => false,
                (Vector2Int c) => Helpers.Distance(entity.Cell, c) > dist);
            return new MoveCommand(entity, cell);
        }
    }
}

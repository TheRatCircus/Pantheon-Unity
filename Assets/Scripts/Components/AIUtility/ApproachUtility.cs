// ApproachUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    public sealed class ApproachUtility : AIUtility
    {
        public override int Recalculate(Entity entity, AI ai)
        {
            if (!ai.Alerted)
                return 0;

            int dst = Helpers.Distance(entity.Cell, Locator.Player.Entity.Cell);

            if (dst > 1)
                return 80;
            else
                return 0;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            Vector2Int cell = entity.Level.PathToPlayer(entity.Cell);
            return new MoveCommand(entity, cell);
        }
    }
}

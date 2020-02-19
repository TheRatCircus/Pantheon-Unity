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
        public int SweetSpot { get; private set; } = 1;

        public override int Recalculate(Entity entity, AI ai)
        {
            if (!ai.Alerted)
                return 0;

            int dist = Helpers.Distance(entity.Cell, Locator.Player.Entity.Cell);

            if (dist < ai.Definition.SweetSpot ||
                dist == ai.Definition.SweetSpot)
                return -1;
            else
                return 80;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            Vector2Int cell = entity.Level.PathToPlayer(entity.Cell);
            return new MoveCommand(entity, cell);
        }
    }
}

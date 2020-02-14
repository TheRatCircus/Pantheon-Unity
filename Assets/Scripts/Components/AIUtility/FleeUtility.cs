// FleeUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Utils;
using Pantheon.World;
using System;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
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
            if (ai.Destination != null)
            {
                if (Helpers.Distance(ai.Target.Cell, ai.Destination) > 20)
                {
                    Vector2Int cell = entity.Level.GetPath(entity.Cell, ai.Destination)[1];
                    return new MoveCommand(entity, cell);
                }
            }

            do
            {
                ai.Destination = entity.Level.RandomCell(
                    (Vector2Int v)
                    => !entity.Level.Walled(v));
            } while (Helpers.Distance(ai.Target.Cell, ai.Destination) < 20);
            Vector2Int c = entity.Level.GetPath(entity.Cell, ai.Destination)[1];
            return new MoveCommand(entity, c);
        }
    }
}

// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using Pantheon.World;
using System;

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
                if (entity.Level.Distance(ai.Target.Cell, ai.Destination) > 20)
                {
                    Cell cell = entity.Level.GetPath(entity.Cell, ai.Destination)[1];
                    return new MoveCommand(entity, cell);
                }
            }

            do
            {
                ai.Destination = entity.Level.RandomCell(true);
            } while (entity.Level.Distance(ai.Target.Cell, ai.Destination) < 20);
            Cell c = entity.Level.GetPath(entity.Cell, ai.Destination)[1];
            return new MoveCommand(entity, c);
        }
    }
}
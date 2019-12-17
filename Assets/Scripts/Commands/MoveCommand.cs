// MoveCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Core;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Commands
{
    public sealed class MoveCommand : ActorCommand
    {
        private Level destinationLevel;
        private Cell destinationCell;

        /// <summary>
        /// Given an AI entity, move to a target only if possible.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ActorCommand MoveOrWait(Entity entity, Cell target)
        {
            List<Cell> path = entity.Level.GetPathTo(entity.Cell, target);

            if (path.Count > 0)
                return new MoveCommand(entity, path[0]);
            else
                return new WaitCommand(entity);
        }

        public MoveCommand(Entity entity, Cell destination)
            : base(entity)
        {
            destinationCell = destination;
            destinationLevel = entity.Level;
        }

        public MoveCommand(Entity actor, Vector2Int dir)
            : base(actor)
        {
            destinationLevel = Entity.Level;

            if (destinationLevel.TryGetCell(Entity.Cell.Position + dir, out Cell c))
                destinationCell = c;
            else
                destinationCell = null;
        }

        public override int Execute()
        {
            if (!Cell.Walkable(destinationCell))
                return -1;

            if (destinationCell.Actor != null)
            {
                Actor actor = Entity.GetComponent<Actor>();
                if (actor.HostileTo(destinationCell.Actor.GetComponent<Actor>()))
                    return new MeleeCommand(Entity, destinationCell).Execute();
                else if (Entity.HasComponent<AI>())
                    return new WaitCommand(Entity).Execute();
            }

            Entity.Move(destinationLevel, destinationCell);
            return TurnScheduler.TurnTime;
        }

        public override string ToString()
        {
            return $"{Entity.ToString()} moving to {destinationCell}";
        }
    }
}

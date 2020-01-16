// MoveCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Core;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;
using ActorComp = Pantheon.Components.Actor;

namespace Pantheon.Commands.Actor
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

        public override CommandResult Execute(out int cost)
        {
            if (!Cell.Walkable(destinationCell))
            {
                cost = -1;
                return CommandResult.Failed;
            }

            if (destinationCell.Actor != null)
            {
                ActorComp actor = Entity.GetComponent<ActorComp>();
                if (actor.HostileTo(destinationCell.Actor.GetComponent<ActorComp>()))
                {
                    ActorCommand cmd = new MeleeCommand(Entity, destinationCell);
                    return cmd.Execute(out cost);
                }
                else if (Entity.HasComponent<AI>())
                {
                    ActorCommand cmd = new WaitCommand(Entity);
                    return cmd.Execute(out cost);
                }
            }

            Entity.Move(destinationLevel, destinationCell);
            cost = TurnScheduler.TurnTime;
            return CommandResult.Succeeded;
        }

        public override string ToString()
        {
            return $"{Entity.ToString()} moving to {destinationCell}";
        }
    }
}

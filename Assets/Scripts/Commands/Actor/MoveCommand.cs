// MoveCommand.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;
using ActorComp = Pantheon.Components.Entity.Actor;

namespace Pantheon.Commands.Actor
{
    public sealed class MoveCommand : ActorCommand
    {
        private Level destinationLevel;
        private Vector2Int destinationCell;

        /// <summary>
        /// Given an AI entity, move to a target only if possible.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ActorCommand MoveOrWait(Entity entity, Vector2Int target)
        {
            Line path = entity.Level.GetPath(entity.Cell, target);

            if (path.Count > 0)
                return new MoveCommand(entity, path[0]);
            else
                return new WaitCommand(entity);
        }

        public MoveCommand(Entity entity, Vector2Int destination)
            : base(entity)
        {
            destinationCell = destination;
            destinationLevel = entity.Level;
        }

        public MoveCommand(Entity actor, int xDirection, int yDirection)
            : base(actor)
        {
            destinationLevel = Entity.Level;
            Vector2Int destination = new Vector2Int(
                Entity.Cell.x + xDirection,
                Entity.Cell.y + yDirection);
            if (destinationLevel.Walkable(destination))
            {
                Cost = TurnScheduler.TurnTime;
                destinationCell = destination;
            }
            else
            {
                Cost = -1;
                destinationCell = Level.NullCell;
            }
                
        }

        public override CommandResult Execute()
        {
            if (destinationCell == Level.NullCell)
            {
                Cost = -1;
                return CommandResult.Failed;
            }

            if (!destinationLevel.Walkable(destinationCell))
            {
                Cost = -1;
                return CommandResult.Failed;
            }

            Entity other = destinationLevel.ActorAt(destinationCell);
            if (other != null)
            {
                ActorComp actor = Entity.GetComponent<ActorComp>();
                if (actor.HostileTo(other.GetComponent<ActorComp>()))
                {
                    ActorCommand cmd = new MeleeCommand(Entity, destinationCell);
                    CommandResult result = cmd.Execute();
                    Cost = cmd.Cost;
                    return result;
                }
                else if (Entity.HasComponent<AI>())
                {
                    ActorCommand cmd = new WaitCommand(Entity);
                    CommandResult result = cmd.Execute();
                    Cost = cmd.Cost;
                    return result;
                }
            }

            Entity.Move(destinationLevel, destinationCell);
            Cost = TurnScheduler.TurnTime;
            return CommandResult.Succeeded;
        }

        public override string ToString()
        {
            return $"{Entity.ToString()} moving to {destinationCell}";
        }
    }
}

// MoveCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Commands
{
    public sealed class MoveCommand : ActorCommand
    {
        public Level DestinationLevel { get; private set; }
        public Cell DestinationCell { get; private set; }
        public int MoveTime { get; private set; }

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
                return new MoveCommand(entity, path[0], TurnScheduler.TurnTime);
            else
                return new WaitCommand(entity);
        }

        public MoveCommand(Entity entity, Cell destination, int moveTime)
            : base(entity, moveTime)
        {
            DestinationCell = destination;
            MoveTime = moveTime;
            DestinationLevel = entity.Level;
        }

        public MoveCommand(Entity actor, Vector2Int dir, int moveTime)
            : base(actor, moveTime)
        {
            MoveTime = moveTime;
            DestinationLevel = Entity.Level;

            if (DestinationLevel.TryGetCell(Entity.Cell.Position + dir, out Cell c))
                DestinationCell = c;
            else
                DestinationCell = null;
        }

        public override void Execute()
        {
            if (DestinationCell == null)
                return;

            if (DestinationCell.Terrain == null)
                return;
                
            if (DestinationCell.Blocked)
            {
                Actor actor = Entity.GetComponent<Actor>();
                if (actor.HostileTo(DestinationCell.Actor.GetComponent<Actor>()))
                    actor.Command = new MeleeCommand(Entity, TurnScheduler.TurnTime, DestinationCell);
                else if (Entity.HasComponent<AI>())
                    actor.Command = new WaitCommand(Entity);

                return;
            }

            Entity.Move(DestinationLevel, DestinationCell);
        }

        public override string ToString()
        {
            return $"{Entity.ToString()} moving to {DestinationCell}";
        }
    }
}

// MoveCommand.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.ECS.Components;
using Pantheon.ECS.Systems;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Commands
{
    public sealed class MoveCommand : Command
    {
        public Level DestinationLevel { get; private set; }
        public Cell DestinationCell { get; private set; }
        public int MoveTime { get; private set; }

        public static Command MoveOrWait(Entity e, Cell target)
        {
            Position pos = e.GetComponent<Position>();
            List<Cell> path = pos.Level.GetPathTo(pos.Cell, target);

            if (path.Count > 0)
                return new MoveCommand(e, path[0], ActorSystem.TurnTime);
            else
                return new WaitCommand(e);
        }

        public MoveCommand(Entity entity, Cell destination, int moveTime)
            : base(entity)
        {
            DestinationCell = destination;
            MoveTime = moveTime;
            DestinationLevel = entity.GetComponent<Position>().Level;
        }

        public MoveCommand(Entity entity, Vector2Int dir, int moveTime)
            : base(entity)
        {
            MoveTime = moveTime;
            Position pos = entity.GetComponent<Position>();
            DestinationLevel = pos.Level;

            if (DestinationLevel.TryGetCell(pos.Cell.Position + dir, out Cell c))
                DestinationCell = c;
            else
                DestinationCell = null;
        }

        public override int Execute()
        {
            if (DestinationCell == null)
                return -1;

            if (DestinationCell.Terrain == null)
                return -1;
                
            if (DestinationCell.Blocked)
            {
                if (Entity.GetComponent<Actor>().HostileTo(DestinationCell.Blocker))
                    return new MeleeCommand(Entity, DestinationCell).Execute();
                else if (Entity.HasComponent<AI>())
                    return new WaitCommand(Entity).Execute();
                else
                    return -1;
            }

            Entity.GetComponent<Position>().SetDestination(DestinationLevel,
                DestinationCell);

            return MoveTime;
        }

        public override string ToString()
        {
            return $"{Entity.ToString()} is moving to {DestinationCell}";
        }
    }
}

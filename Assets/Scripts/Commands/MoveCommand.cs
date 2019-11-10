// MoveCommand.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.Commands
{
    public sealed class MoveCommand : Command
    {
        public Level DestinationLevel { get; private set; }
        public Cell DestinationCell { get; private set; }
        public int MoveTime { get; private set; }

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

            if (DestinationLevel.Map.TryGetValue(pos.Cell.Position + dir, out Cell c))
                DestinationCell = c;
            else
                DestinationCell = null;
        }

        public override int Execute()
        {
            if (DestinationCell == null)
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

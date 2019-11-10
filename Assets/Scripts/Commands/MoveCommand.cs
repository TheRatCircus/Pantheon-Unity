// MoveCommand.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.Commands
{
    public sealed class MoveCommand : Command
    {
        public Level Level { get; private set; }
        public Cell Destination { get; private set; }
        public int MoveTime { get; private set; }

        public MoveCommand(Entity entity, Cell destination, int moveTime)
            : base(entity)
        {
            Destination = destination;
            MoveTime = moveTime;
            Level = entity.GetComponent<Position>().Level;
        }

        public MoveCommand(Entity entity, Vector2Int dir, int moveTime)
            : base(entity)
        {
            MoveTime = moveTime;
            Level = entity.GetComponent<Position>().Level;
            if (!Level.Map.TryGetValue(dir, out Cell c))
                Destination = null;
            else
                Destination = c;
        }

        public override int Execute()
        {
            if (Destination == null)
                return -1;

            if (Destination.Blocked)
            {
                if (Entity.GetComponent<Actor>().HostileTo(Destination.Blocker))
                    return new MeleeCommand(Entity, Destination).Execute();
                else if (Entity.HasComponent<AI>())
                    return new WaitCommand(Entity).Execute();
                else
                    return -1;
            }
            Entity.GetComponent<Position>().Move(Entity, Level, Destination);
            return MoveTime;
        }
    }
}

// MoveCommand.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;
using ActorComp = Pantheon.Components.Actor;

namespace Pantheon.Commands.Actor
{
    public sealed class MoveCommand : ActorCommand
    {
        private readonly Level destinationLevel;
        private readonly Vector2Int destinationCell;

        /// <summary>
        /// Given an AI entity, move to a target only if possible.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ActorCommand MoveOrWait(Entity entity, Vector2Int target)
        {
            Line path = entity.Level.GetPathTo(entity.Position, target);

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

        public static MoveCommand Directional(Entity entity, Vector2Int dir)
        {
            return new MoveCommand(entity, entity.Position + dir);
        }

        public override CommandResult Execute(out int cost)
        {
            if (!destinationLevel.Walkable(destinationCell))
            {
                cost = -1;
                return CommandResult.Failed;
            }

            Entity e = destinationLevel.ActorAt(destinationCell);
            if (e != null)
            {
                ActorComp actor = Entity.GetComponent<ActorComp>();
                if (actor.HostileTo(e.GetComponent<ActorComp>()))
                {
                    ActorCommand cmd = new MeleeCommand(Entity, destinationCell);
                    return cmd.Execute(out cost);
                }
                else 
                if (Entity.HasComponent<AI>())
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

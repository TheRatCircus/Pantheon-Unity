// MoveCommand.cs
// Jerome Martina

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
        /// <param name="actor"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ActorCommand MoveOrWait(Actor actor, Cell target)
        {
            List<Cell> path = actor.Level.GetPathTo(actor.Cell, target);

            if (path.Count > 0)
                return new MoveCommand(actor, path[0], TurnScheduler.TurnTime);
            else
                return new WaitCommand(actor);
        }

        public MoveCommand(Actor actor, Cell destination, int moveTime)
            : base(actor, moveTime)
        {
            DestinationCell = destination;
            MoveTime = moveTime;
            DestinationLevel = Actor.Level;
        }

        public MoveCommand(Actor actor, Vector2Int dir, int moveTime)
            : base(actor, moveTime)
        {
            MoveTime = moveTime;
            DestinationLevel = Actor.Level;

            if (DestinationLevel.TryGetCell(Actor.Cell.Position + dir, out Cell c))
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
                if (Actor.HostileTo(DestinationCell.Actor))
                    Actor.Command = new MeleeCommand(Actor, TurnScheduler.TurnTime, DestinationCell);
                else if (Actor.GetComponent<AI>())
                    Actor.Command = new WaitCommand(Actor);

                return;
            }

            Actor.Move(DestinationLevel, DestinationCell);
        }

        public override string ToString()
        {
            return $"{Actor.ToString()} moving to {DestinationCell}";
        }
    }
}

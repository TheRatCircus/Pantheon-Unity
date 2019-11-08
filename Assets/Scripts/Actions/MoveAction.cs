// MoveAction.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Actions
{
    /// <summary>
    /// An attempt by a creature to move into a new cell.
    /// </summary>
    public sealed class MoveAction : Command
    {
        private Cell destination;
        private Cell previous;

        private readonly int moveTime;

        // Cell-based constructor
        public MoveAction(Actor actor, int moveTime, Cell destination)
            : base(actor)
        {
            this.moveTime = moveTime;

            previous = actor.Cell;
            this.destination = destination;
        }

        // Direction-based constructor
        public MoveAction(Actor actor, int moveTime, Vector2Int dir)
            : base(actor)
        {
            this.moveTime = moveTime;

            previous = actor.Cell;
            destination = actor.level.GetCell(actor.Cell.Position + dir);
        }

        // Try to move to the destination cell
        public override int DoAction()
        {
            if (destination == null)
                UnityEngine.Debug.LogException(new System.Exception
                    ("A MoveAction was initialized with a null cell"));

            if (destination.Blocked)
                return -1;

            if (destination.Actor != null)
            {
                if (Actor.IsHostileTo(destination.Actor))
                    return new MeleeAction(Actor, destination).DoAction();
                else
                    return new WaitAction(Actor).DoAction();
            }

            Actor.MoveTo(Actor, destination);

            // Empty previous cell if one exists
            if (previous != null)
                previous.Actor = null;

            return moveTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();

        public override string ToString()
            => $"{Actor.ActorName} is moving from" +
            $" {previous.Position} to {destination.Position}.";
    }
}

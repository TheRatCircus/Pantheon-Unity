// MoveAction.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;
using Pantheon.World;

namespace Pantheon.Actions
{
    /// <summary>
    /// An attempt by a creature to move into a new cell.
    /// </summary>
    public class MoveAction : BaseAction
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
                Debug.LogException(new System.Exception("A MoveAction was initialized with a null cell"));

            if (!destination.IsWalkableTerrain())
                return -1;

            if (destination.Actor != null)
            {
                if (Actor.HostileToMe(destination.Actor))
                    Actor.NextAction = new MeleeAction(Actor, destination.Actor);
                else
                    Actor.NextAction = new WaitAction(Actor);

                return -1;
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
    }
}

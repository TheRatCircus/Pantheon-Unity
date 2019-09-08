// Move a creature to a new cell

using UnityEngine;
using Pantheon.Actors;
using Pantheon.World;

namespace Pantheon.Actions
{
    public class MoveAction : BaseAction
    {
        public Cell Destination;
        public Cell Previous;

        public int MoveTime;

        // Cell-based constructor
        public MoveAction(Actor actor, int moveTime, Cell destination)
            : base(actor)
        {
            MoveTime = moveTime;

            Previous = actor.Cell;
            Destination = destination;
        }

        // Direction-based constructor
        public MoveAction(Actor actor, int moveTime, Vector2Int dir)
            : base(actor)
        {
            MoveTime = moveTime;

            Previous = actor.Cell;
            Destination = actor.level.GetCell(actor.Cell.Position + dir);
        }

        // Try to move to the destination cell
        public override int DoAction()
        {
            if (Destination == null)
                Debug.LogException(new System.Exception("A MoveAction was initialized with a null cell"));

            if (!Destination.IsWalkableTerrain())
                return -1;

            if (Destination._actor != null)
            {
                if (Actor.HostileToMe(Destination._actor))
                    Actor.nextAction = new MeleeAction(Actor, Actor.attackTime, Destination._actor);
                else
                    Actor.nextAction = new WaitAction(Actor);

                return -1;
            }

            Actor.MoveTo(Actor, Destination);

            // Empty previous cell if one exists
            if (Previous != null)
                Previous._actor = null;

            return MoveTime;
        }

        // DoAction() with a callback
        public override int DoAction(OnConfirm onConfirm)
            => throw new System.NotImplementedException();
    }
}

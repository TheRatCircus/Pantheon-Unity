// DefaultStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Components
{
    /// <summary>
    /// Basic enemy strategy. Move to player and melee.
    /// </summary>
    [System.Serializable]
    public sealed class DefaultStrategy : AIStrategy
    {
        private Entity target;

        public DefaultStrategy() { }

        public DefaultStrategy(Entity target) => SetTarget(target);

        public void SetTarget(Entity target)
        {
            this.target = target;
            target.DestroyedEvent += ClearTarget;
        }

        private void ClearTarget()
        {
            target.DestroyedEvent -= ClearTarget;
            target = null;
        }

        public override ActorCommand Decide(AI ai)
        {
            // Random energy
            int r = Random.Range(0, 21);
            if (r >= 18)
                ai.Actor.Energy += ai.Actor.Speed / 10;
            else if (r <= 2)
                ai.Actor.Energy -= ai.Actor.Speed / 10;

            if (target != null) // Player detected
            {
                Cell targetCell = target.Cell;

                if (ai.Entity.Level.AdjacentTo(ai.Entity.Cell, targetCell))
                    return new MeleeCommand(ai.Entity, targetCell);
                else
                    return MoveCommand.MoveOrWait(ai.Entity, targetCell);
            }
            else
            {
                ai.Strategy = new SleepStrategy();
                return ai.Strategy.Decide(ai);
            }
        }
    }
}

// ThrallFollowStrategy.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Components
{
    /// <summary>
    /// Follow a designated master and attack any hostiles coming close to it.
    /// </summary>
    public sealed class ThrallFollowStrategy : AIStrategy
    {
        private Actor master;
        public Actor Master { get => master; set => master = value; }

        public ThrallFollowStrategy() { }

        public ThrallFollowStrategy(Actor master) => this.master = master;

        public override ActorCommand Decide(AI ai)
        {
            // Random energy
            int r = Random.Range(0, 21);
            if (r >= 18)
                ai.Actor.Energy += ai.Actor.Speed / 10;
            else if (r <= 2)
                ai.Actor.Energy -= ai.Actor.Speed / 10;

            Cell enemyCell = ai.Entity.Level.FindNearest( // Find nearest enemy
                master.Entity.Cell, 5, delegate (Cell cell)
                {
                    if (cell.Actor == null)
                        return false;
                    else
                        return cell.Actor.GetComponent<Actor>().HostileTo(master);
                });

            if (enemyCell == null) // Move to or wait next to master
            {
                if (ai.Entity.Level.AdjacentTo(ai.Entity.Cell, master.Entity.Cell))
                    return new WaitCommand(ai.Entity);
                else
                    return MoveCommand.MoveOrWait(ai.Entity, master.Entity.Cell);
            }
            else // Move to engage with enemy
            {
                if (ai.Entity.Level.AdjacentTo(ai.Entity.Cell, enemyCell))
                    return new MeleeCommand(ai.Entity, enemyCell);
                else
                    return MoveCommand.MoveOrWait(ai.Entity, enemyCell);
            }
        }
    }
}

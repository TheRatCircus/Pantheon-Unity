// ThrallFollowStrategy.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

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
            Vector2Int enemyCell = Floodfill.QueueFillForCell(
                ai.Entity.Level,
                ai.Entity.Cell,
                (Vector2Int v) => Helpers.Distance(ai.Entity.Cell, v) > 15,
                delegate (Vector2Int c) 
                {
                    Entity enemy = ai.Entity.Level.ActorAt(c);
                    return enemy != null && enemy.GetComponent<Actor>().HostileTo(master);
                });
                
            if (enemyCell == null) // Move to or wait next to master
            {
                if (Helpers.Adjacent(ai.Entity.Cell, master.Entity.Cell))
                    return new WaitCommand(ai.Entity);
                else
                    return MoveCommand.MoveOrWait(ai.Entity, master.Entity.Cell);
            }
            else // Move to engage with enemy
            {
                if (Helpers.Adjacent(ai.Entity.Cell, enemyCell))
                    return new MeleeCommand(ai.Entity, enemyCell);
                else
                    return MoveCommand.MoveOrWait(ai.Entity, enemyCell);
            }
        }
    }
}

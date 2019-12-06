// AI.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class AI : EntityComponent
    {
        private static Entity player;
        private static GameLog log;

        public Entity Entity { get; set; }
        public Actor Actor { get; set; }
        public Entity Target { get; set; }

        public static void Init(Entity player, GameLog log)
        {
            AI.player = player;
            AI.log = log;
        }

        public void DecideCommand()
        {
            // Random energy
            int r = Random.Range(0, 21);
            if (r >= 18)
                Actor.Energy += Actor.Speed / 10;
            else if (r <= 2)
                Actor.Energy -= Actor.Speed / 10;

            if (Target != null) // Player detected
            {
                Cell targetCell = Entity.Cell;

                if (Entity.Level.AdjacentTo(Entity.Cell, targetCell))
                    Actor.Command = new MeleeCommand(Entity, TurnScheduler.TurnTime, targetCell);
                else
                    Actor.Command = MoveCommand.MoveOrWait(Entity, targetCell);
            }
            else // Player not encountered yet
            {
                if (Entity.Cell.Visible) // Detect player and begin approach
                {
                    Target = player;
                    Actor.Command = MoveCommand.MoveOrWait(Entity, Target.Cell);
                    log.Send($"The {Entity.Name} notices you!", Colours._orange);
                }
                else // Sleep
                    Actor.Command = new WaitCommand(Entity);
            }
        }
    }
}

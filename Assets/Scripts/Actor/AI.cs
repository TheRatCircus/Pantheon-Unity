// AI.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public class AI : MonoBehaviour
    {
        private static Actor player;
        private static GameLog log;

        public Actor Actor { get; set; }
        public Actor Target { get; set; }

        public static void Init(Actor player, GameLog log)
        {
            AI.player = player;
            AI.log = log;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Actor = GetComponent<Actor>();
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
                Cell targetCell = Actor.Cell;

                if (Actor.Level.AdjacentTo(Actor.Cell, targetCell))
                    Actor.Command = new MeleeCommand(Actor, TurnScheduler.TurnTime, targetCell);
                else
                    Actor.Command = MoveCommand.MoveOrWait(Actor, targetCell);
            }
            else // Player not encountered yet
            {
                if (Actor.Cell.Visible) // Detect player and begin approach
                {
                    Target = player;
                    Actor.Command = MoveCommand.MoveOrWait(Actor, Target.Cell);
                    log.Send($"The {Actor.Name} notices you!", Colours._orange);
                }
                else // Sleep
                    Actor.Command = new WaitCommand(Actor);
            }
        }
    }
}

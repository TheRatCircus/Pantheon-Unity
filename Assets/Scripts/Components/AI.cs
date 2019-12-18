// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Newtonsoft.Json;
using Pantheon.Commands;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class AI : EntityComponent
    {
        private static GameController ctrl;

        [JsonIgnore] public Entity Entity { get; set; }
        [JsonIgnore] public Actor Actor { get; private set; }
        [JsonIgnore] public Entity Target { get; set; }

        public static void InjectController(GameController ctrl)
        {
            AI.ctrl = ctrl;
        }

        public void SetActor(Actor actor)
        {
            Actor = actor;
            actor.AIDecisionEvent += DecideCommand;
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
                Cell targetCell = Target.Cell;

                if (Entity.Level.AdjacentTo(Entity.Cell, targetCell))
                    Actor.Command = new MeleeCommand(Entity, targetCell);
                else
                    Actor.Command = MoveCommand.MoveOrWait(Entity, targetCell);
            }
            else // Player not encountered yet
            {
                if (Entity.Cell.Visible) // Detect player and begin approach
                {
                    Target = ctrl.Player;
                    Actor.Command = MoveCommand.MoveOrWait(Entity, Target.Cell);
                    LogLocator.Service.Send(
                        $"{Entity.ToSubjectString(true)} notices you!",
                        Colours._orange);
                }
                else
                    Actor.Command = new WaitCommand(Entity); // Sleep
            }
            DebugLogAI();
        }

        public override EntityComponent Clone() => new AI();

        [System.Diagnostics.Conditional("DEBUG_AI")]
        private void DebugLogAI()
        {
            UnityEngine.Debug.Log($"{Entity} command: {Actor.Command}");
        }
    }
}

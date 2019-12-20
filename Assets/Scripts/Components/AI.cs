// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Newtonsoft.Json;
using Pantheon.Commands;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class AI : EntityComponent
    {
        [JsonIgnore] public Actor Actor { get; private set; }

        public AIStrategy Strategy { get; set; } = new DefaultStrategy();

        public AI(AIStrategy strategy) => Strategy = strategy;

        public void SetActor(Actor actor)
        {
            Actor = actor;
            actor.AIDecisionEvent += DecideCommand;
        }
        
        public void DecideCommand()
        {
            Actor.Command = Strategy.Decide(this);
            DebugLogAI();
        }

        public override EntityComponent Clone(bool full) => new AI(Strategy);

        [System.Diagnostics.Conditional("DEBUG_AI")]
        private void DebugLogAI()
        {
            UnityEngine.Debug.Log($"{Entity} command: {Actor.Command}");
        }
    }

    [System.Serializable]
    public abstract class AIStrategy
    {
        public abstract ActorCommand Decide(AI ai);
    }

    /// <summary>
    /// Basic enemy strategy. Move to player and melee.
    /// </summary>
    [System.Serializable]
    public sealed class DefaultStrategy : AIStrategy
    {
        private Entity target;

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
            else // Player not encountered yet
            {
                if (ai.Entity.Cell.Visible) // Detect player and begin approach
                {
                    target = InputLocator.Service.PlayerEntity;
                    LogLocator.Service.Send(
                        $"{ai.Entity.ToSubjectString(true)} notices you!",
                        Colours._orange);
                    return MoveCommand.MoveOrWait(ai.Entity, target.Cell);
                }
                else
                    return new WaitCommand(ai.Entity); // Sleep
            }
        }
    }
}

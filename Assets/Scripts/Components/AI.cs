// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Newtonsoft.Json;
using Pantheon.Utils;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class AI : EntityComponent
    {
        [JsonIgnore] public Actor Actor { get; private set; }

        public AIStrategy Strategy { get; set; } = new SleepStrategy();

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

        public override void Receive(IComponentMessage msg)
        {
            switch (msg)
            {
                case DamageEventMessage dem:
                    Strategy = new DefaultStrategy(dem.Damager);
                    LogLocator.Service.Send(
                        $"{Entity.ToSubjectString(true)} notices you!",
                        Colours._orange);
                    break;
                default:
                    break;
            }
        }

        public override EntityComponent Clone(bool full) => new AI(Strategy);

        [System.Diagnostics.Conditional("DEBUG_AI")]
        private void DebugLogAI()
        {
            UnityEngine.Debug.Log($"{Entity} command: {Actor.Command}");
        }
    }
}

// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Newtonsoft.Json;
using Pantheon.Utils;

namespace Pantheon.Components.Entity
{
    [System.Serializable]
    public sealed class AI : EntityComponent
    {
        [JsonIgnore] public Actor Actor { get; private set; }

        public AIStrategy Strategy { get; set; }

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
                    if (!(Strategy is ThrallFollowStrategy))
                    {
                        if (Strategy is DefaultStrategy ds && ds.Target != dem.Damager)
                            ds.SetTarget(dem.Damager);
                        else
                            Strategy = new DefaultStrategy(dem.Damager);

                        Locator.Log.Send(
                            $"{Entity.ToSubjectString(true)} notices you!",
                            Colours._orange);
                    }
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

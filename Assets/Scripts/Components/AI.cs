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

        public AI() { }

        public void SetActor(Actor actor)
        {
            Actor = actor;
            actor.AIDecisionEvent += DecideCommand;
        }

        public void DecideCommand()
        {
            throw new System.NotImplementedException();
            DebugLogAI();
        }

        public override void Receive(IComponentMessage msg)
        {
            switch (msg)
            {
                case DamageEventMessage _:
                    Locator.Log.Send(
                        $"{Entity.ToSubjectString(true)} notices you!",
                        Colours._orange);
                    break;
                default:
                    break;
            }
        }

        public override EntityComponent Clone(bool full) => new AI();

        [System.Diagnostics.Conditional("DEBUG_AI")]
        private void DebugLogAI()
        {
            UnityEngine.Debug.Log($"{Entity} command: {Actor.Command}");
        }
    }
}

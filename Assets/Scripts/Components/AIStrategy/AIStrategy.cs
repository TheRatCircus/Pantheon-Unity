// AIStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;

namespace Pantheon.Components.Entity
{
    [System.Serializable]
    public abstract class AIStrategy
    {
        public abstract ActorCommand Decide(AI ai);
    }
}

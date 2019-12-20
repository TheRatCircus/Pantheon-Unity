// AIStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands;

namespace Pantheon.Components
{
    [System.Serializable]
    public abstract class AIStrategy
    {
        public abstract ActorCommand Decide(AI ai);
    }
}

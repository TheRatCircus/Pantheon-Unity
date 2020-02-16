// SleepStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;

namespace Pantheon.Components.Entity
{
    /// <summary>
    /// Causes NPC to wait in place.
    /// </summary>
    [System.Serializable]
    public sealed class SleepStrategy : AIStrategy
    {
        public override ActorCommand Decide(AI ai)
        {
            throw new System.NotImplementedException();
        }
    }
}

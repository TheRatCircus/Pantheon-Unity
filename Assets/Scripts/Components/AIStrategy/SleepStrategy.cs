// SleepStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands;
using Pantheon.Utils;

namespace Pantheon.Components
{
    /// <summary>
    /// Causes NPC to wait in place.
    /// </summary>
    [System.Serializable]
    public sealed class SleepStrategy : AIStrategy
    {
        public override ActorCommand Decide(AI ai)
        {
            if (ai.Entity.Cell.Visible) // Detect player and begin approach
            {
                ai.Strategy = new DefaultStrategy(InputLocator.Service.PlayerEntity);
                LogLocator.Service.Send(
                    $"{ai.Entity.ToSubjectString(true)} notices you!",
                    Colours._orange);
                return MoveCommand.MoveOrWait(
                    ai.Entity, InputLocator.Service.PlayerEntity.Cell);
            }
            else
                return new WaitCommand(ai.Entity);
        }
    }
}

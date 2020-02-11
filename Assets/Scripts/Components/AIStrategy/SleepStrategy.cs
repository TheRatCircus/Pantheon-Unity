// SleepStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using Pantheon.Utils;

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
            if (ai.Entity.Cell.Visible) // Detect player and begin approach
            {
                ai.Strategy = new DefaultStrategy(Locator.Player.Entity);
                Locator.Log.Send(
                    $"{Strings.Subject(ai.Entity, true)} notices you!",
                    Colours._orange);
                return MoveCommand.MoveOrWait(
                    ai.Entity, Locator.Player.Entity.Cell);
            }
            else
                return new WaitCommand(ai.Entity);
        }
    }
}

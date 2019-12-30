// WanderStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using Pantheon.Utils;
using Pantheon.World;

namespace Pantheon.Components
{
    /// <summary>
    /// Wander at random throughout the level.
    /// </summary>
    [System.Serializable]
    public sealed class WanderStrategy : AIStrategy
    {
        private Cell destination;

        public override ActorCommand Decide(AI ai)
        {
            if (ai.Entity.Cell.Visible) // Detect player and begin approach
            {
                ai.Strategy = new DefaultStrategy(Locator.Player.Entity);
                Locator.Log.Send(
                    $"{ai.Entity.ToSubjectString(true)} notices you!",
                    Colours._orange);
                return ai.Strategy.Decide(ai);
            }

            if (ai.Entity.Cell != destination)
                destination = ai.Entity.Level.RandomCell(true);

            return MoveCommand.MoveOrWait(ai.Entity, destination);
        }
    }
}

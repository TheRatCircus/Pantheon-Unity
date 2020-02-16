// DefaultStrategy.cs
// Jerome Martina

using Pantheon.Commands.Actor;

namespace Pantheon.Components.Entity
{
    /// <summary>
    /// Basic enemy strategy. Move to player and melee.
    /// </summary>
    [System.Serializable]
    public sealed class DefaultStrategy : AIStrategy
    {
        public DefaultStrategy() { }

        public override ActorCommand Decide(AI ai)
        {
            throw new System.NotImplementedException();
        }
    }
}

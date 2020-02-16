// WanderStrategy.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    /// <summary>
    /// Wander at random throughout the level.
    /// </summary>
    [System.Serializable]
    public sealed class WanderStrategy : AIStrategy
    {
        private Vector2Int destination;

        public override ActorCommand Decide(AI ai)
        {
            throw new System.NotImplementedException();
        }
    }
}

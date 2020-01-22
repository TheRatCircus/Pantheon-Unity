// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Pantheon.Commands.Actor;
using System;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public abstract class AIUtility
    {
        public abstract int Recalculate(Entity entity, AI ai);
        public abstract ActorCommand Invoke(Entity entity, AI ai);
    }
}
// AIUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    public abstract class AIUtility
    {
        public abstract int Recalculate(Entity entity, AI ai);
        public abstract ActorCommand Invoke(Entity entity, AI ai);
    }
}

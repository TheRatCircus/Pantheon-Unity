// ComponentSystem.cs
// Jerome Martina

using Pantheon.UI;

namespace Pantheon.ECS.Systems
{
    public abstract class ComponentSystem
    {
        protected EntityManager mgr;
        protected GameLog log;

        public ComponentSystem(EntityManager mgr, GameLog log)
        {
            this.mgr = mgr;
            this.log = log;
        }

        public abstract void Update();
    }
}
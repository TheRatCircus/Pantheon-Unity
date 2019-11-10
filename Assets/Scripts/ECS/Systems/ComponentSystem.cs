// ComponentSystem.cs
// Jerome Martina

namespace Pantheon.ECS.Systems
{
    public abstract class ComponentSystem
    {
        protected EntityManager mgr;

        public ComponentSystem(EntityManager mgr) => this.mgr = mgr;

        public abstract void UpdateComponents();
    }
}

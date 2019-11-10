// ComponentSystem.cs
// Jerome Martina

namespace Pantheon.ECS.Systems
{
    public abstract class ComponentSystem
    {
        protected Manager mgr;

        public abstract void UpdateComponents();
    }
}

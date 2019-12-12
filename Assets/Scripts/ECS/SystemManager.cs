// SystemManager.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS.Systems;
using Pantheon.UI;

namespace Pantheon.ECS
{
    public sealed class SystemManager
    {
        private ComponentSystem[] systems;

        private AISystem aiSystem;

        public SystemManager(EntityManager mgr, GameLog log, TurnScheduler scheduler)
        {
            systems = new ComponentSystem[]
            {
                new LocationSystem(mgr, log)
            };
            aiSystem = new AISystem(mgr, log, scheduler);
        }

        public void Update()
        {
            foreach (ComponentSystem sys in systems)
                sys.Update();
        }
    }
}

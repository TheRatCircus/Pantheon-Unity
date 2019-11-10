// SystemManager.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Systems
{
    public sealed class SystemManager : MonoBehaviour
    {
        public PlayerSystem PlayerSystem { get; private set; }
        private ActorSystem actorSystem;

        private ComponentSystem[] systems = new ComponentSystem[2];

        public void Initialize(EntityManager mgr)
        {
            PlayerSystem = new PlayerSystem(mgr);
            actorSystem = new ActorSystem(mgr);
            actorSystem.ActionDoneEvent += UpdatePerTurnSystems;

            systems[0] = new HealthSystem(mgr);
            systems[1] = new PositionSystem(mgr);
        }

        private void Update() => UpdatePerFrameSystems();

        private void UpdatePerFrameSystems()
        {
            PlayerSystem.UpdateComponents();
            actorSystem.UpdateComponents();
        }

        public void UpdatePerTurnSystems()
        {
            foreach (ComponentSystem sys in systems)
                sys.UpdateComponents();
        }
    }
}

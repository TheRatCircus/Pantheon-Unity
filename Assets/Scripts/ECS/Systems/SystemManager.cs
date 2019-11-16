// SystemManager.cs
// Jerome Martina

using UnityEngine;
using Cursor = Pantheon.UI.Cursor;

namespace Pantheon.ECS.Systems
{
    public sealed class SystemManager : MonoBehaviour
    {
        [SerializeField] private Cursor cursor;
        private PlayerSystem playerSystem;
        public ActorSystem ActorSystem { get; private set; }

        private ComponentSystem[] systems = new ComponentSystem[2];

        public void Initialize(EntityManager mgr)
        {
            playerSystem = new PlayerSystem(mgr, cursor);
            ActorSystem = new ActorSystem(mgr);
            ActorSystem.ActionDoneEvent += UpdatePerTurnSystems;

            systems[0] = new HealthSystem(mgr);
            systems[1] = new PositionSystem(mgr);
        }

        private void Update() => UpdatePerFrameSystems();

        private void UpdatePerFrameSystems()
        {
            playerSystem.UpdateComponents();
            ActorSystem.UpdateComponents();
        }

        public void UpdatePerTurnSystems()
        {
            foreach (ComponentSystem sys in systems)
                sys.UpdateComponents();
        }

        public T GetSystem<T>() where T : ComponentSystem
        {
            if (typeof(T) == typeof(PlayerSystem))
                return playerSystem as T;
            else if (typeof(T) == typeof(ActorSystem))
                return ActorSystem as T;
            else
            {
                foreach (ComponentSystem sys in systems)
                    if (sys is T)
                        return sys as T;
            }
            throw new System.ArgumentException(
                $"Component system of type {typeof(T)} not found.");
        }
    }
}

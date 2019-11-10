// SystemManager.cs
// Jerome Martina

using UnityEngine;
using System.Collections.Generic;

namespace Pantheon.ECS.Systems
{
    public sealed class SystemManager : MonoBehaviour
    {
        private PlayerSystem playerSystem;
        private ActorSystem actorSystem;

        HashSet<ComponentSystem> systems;

        private void Awake()
        {
            actorSystem.ActionDoneEvent += UpdatePerTurnSystems;
        }

        private void Update() => UpdatePerFrameSystems();

        private void UpdatePerFrameSystems()
        {
            playerSystem.UpdateComponents();
            actorSystem.UpdateComponents();
        }

        private void UpdatePerTurnSystems()
        {
            foreach (ComponentSystem sys in systems)
                sys.UpdateComponents();
        }
    }
}

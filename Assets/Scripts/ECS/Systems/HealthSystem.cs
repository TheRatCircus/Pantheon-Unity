// HealthSystem.cs
// Jerome Martina

using UnityEngine;
using Pantheon.ECS.Components;

namespace Pantheon.ECS.Systems
{
    public sealed class HealthSystem : ComponentSystem
    {
        public override void UpdateComponents()
        {
            foreach (Health health in mgr.HealthComponents)
            {
                health.RegenProgress += ActorSystem.TurnTime;
                if (health.RegenProgress >= health.RegenRate)
                {
                    health.Current += 1;
                    health.Current = Mathf.Clamp(health.Current, 0, health.Max);
                }
            }
        }
    }
}

// HealthSystem.cs
// Jerome Martina

using UnityEngine;
using Pantheon.ECS.Components;

namespace Pantheon.ECS.Systems
{
    public sealed class HealthSystem : ComponentSystem
    {
        public HealthSystem(EntityManager mgr) : base(mgr) { }

        public override void UpdateComponents()
        {
            //foreach (Health health in mgr.HealthComponents)
            //{
            //    health.RegenProgress += ActorSystem.TurnTime;
            //    if (health.RegenProgress >= health.RegenRate)
            //    {
            //        health.Current += 1;
            //        health.Current = Mathf.Clamp(health.Current, 0, health.Max);
            //    }
            //}
            foreach (Entity e in mgr.ActiveEntities)
            {
                if (!e.TryGetComponent(out Health health))
                    continue;

                health.RegenProgress += ActorSystem.TurnTime;
                if (health.RegenProgress >= health.RegenRate)
                {
                    health.Current += 1;
                    health.Current = Mathf.Clamp(health.Current, 0, health.Max);
                    health.RegenProgress %= health.RegenRate;
                }
            }
        }
    }
}

// HUD.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.UI
{
    public sealed class HUD : MonoBehaviour
    {
        [SerializeField] private HUDHealth health = default;
        [SerializeField] private HUDEnergy energy = default;
        [SerializeField] private HUDClock clock = default;
        [SerializeField] private HUDLocation location = default;

        public void Initialize(TurnScheduler scheduler, Entity player,
            Level activeLevel, System.Action<Level> levelChangeEvent)
        {
            health.Initialize(player);
            energy.Initialize(player);
            clock.Initialize(scheduler);
            location.Initialize(activeLevel, levelChangeEvent);
        }
    }
}

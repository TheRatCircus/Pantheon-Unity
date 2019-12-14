// HUD.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.UI
{
    public sealed class HUD : MonoBehaviour
    {
        [SerializeField] private HUDHealth health = default;
        [SerializeField] private HUDEnergy energy = default;

        public void Initialize(Entity player)
        {
            health.Initialize(player);
            energy.Initialize(player);
        }
    }
}

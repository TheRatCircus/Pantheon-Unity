// HUD.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.UI
{
    public sealed class HUD : MonoBehaviour
    {
        [SerializeField] private HUDHealth health = default;

        public void Initialize(Entity player)
        {
            health.Initialize(player);
        }
    }
}

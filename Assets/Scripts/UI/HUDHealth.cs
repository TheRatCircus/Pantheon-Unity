// HUDHealth.cs
// Jerome Martina

using Pantheon.Components.Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public sealed class HUDHealth : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hpCounter = default;
        [SerializeField] private Image hpBar = default;

        public void Initialize(Entity player)
        {
            Health health = player.GetComponent<Health>();
            health.HealthChangeEvent += Redraw;
            health.MaxHealthChangeEvent += Redraw;
            Redraw(health);
        }

        private void Redraw(Health health)
        {
            hpCounter.text = $"Health: {health.Current}/{health.Max}";
            hpBar.fillAmount = (float)health.Current / health.Max;
        }
    }
}

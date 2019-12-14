// HUDEnergy.cs
// Jerome Martina

using Pantheon.Components;
using TMPro;
using UnityEngine;

namespace Pantheon.UI
{
    public sealed class HUDEnergy : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI energyCounter = default;

        public void Initialize(Entity player)
        {
            Actor actor = player.GetComponent<Actor>();
            actor.EnergyChangedEvent += Redraw;
            actor.SpeedChangedEvent += Redraw;
        }

        private void Redraw(Actor actor)
        {
            energyCounter.text = $"Energy: {actor.Energy}/{actor.Speed}";
        }
    }
}

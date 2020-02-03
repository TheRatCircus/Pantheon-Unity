// HUDClock.cs
// Jerome Martina

using Pantheon.Core;
using TMPro;
using UnityEngine;

namespace Pantheon.UI
{
    public sealed class HUDClock : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeCounter = default;

        public void Initialize(TurnScheduler scheduler)
        {
            scheduler.TimeChangeEvent += Redraw;
            Redraw(scheduler.Time, 0);
        }

        private void Redraw(int time, int timePassed)
        {
            timeCounter.text = $"Time: {time} ({timePassed})";
        }
    }
}

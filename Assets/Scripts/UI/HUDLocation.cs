// HUDLocation.cs
// Jerome Martina

using Pantheon.World;
using System;
using TMPro;
using UnityEngine;

namespace Pantheon.UI
{
    public sealed class HUDLocation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI location = default;

        public void Initialize(Level activeLevel, Action<Level> levelChangeEvent)
        {
            levelChangeEvent += Redraw;
            Redraw(activeLevel);
        }

        private void Redraw(Level level)
        {
            location.text = $"Location: {level.DisplayName}";
        }
    }
}

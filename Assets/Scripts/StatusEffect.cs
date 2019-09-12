// StatusEffect.cs
// Jerome Martina

using System;
using UnityEngine;
using Pantheon.Utils;

namespace Pantheon.Actors
{
    [System.Serializable]
    public class StatusEffect
    {
        [SerializeField] [ReadOnly] private string displayName;
        [SerializeField] private int turnsRemaining = 10;
        public StatusType Type { get; set; }
        public Strings.TextColour LabelColour { get; set; } // Name colour on HUD

        public string DisplayName { get => displayName; }
        public int TurnsRemaining
        {
            get => turnsRemaining; set => turnsRemaining = value;
        }

        public delegate string StatusEffectDelegate(Actor actor);
        public StatusEffectDelegate OnApply;
        public StatusEffectDelegate OnExpire;

        public StatusEffect(
            string displayName, StatusType type,
            Strings.TextColour colour,
            StatusEffectDelegate onApply,
            StatusEffectDelegate onExpire)
        {
            this.displayName = displayName;
            Type = type;
            LabelColour = colour;
            OnApply = onApply;
            OnExpire = onExpire;
        }

        public override string ToString() => Type.ToString();
    }
}
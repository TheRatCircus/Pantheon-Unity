// Health.cs
// Jerome Martina

using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Pantheon.Components
{
    [Serializable]
    public sealed class Health : EntityComponent
    {
        [SerializeField] private int max = 10;
        [SerializeField] private int regenTime = 800; // Time in ticks to regen 1 HP

        [JsonIgnore] public int Current { get; private set; }
        public int Max
        {
            get => max;
            set
            {
                int prev = value;
                max = value;
                MaxHealthChangeEvent?.Invoke(this);
            }
        }
        public int RegenTime { get => regenTime; set => regenTime = value; }
        [JsonIgnore] public int RegenProgress { get; set; } // Ticks down from regenTime until 0
        public bool Regenerating { get; set; } = true; // Status, not intrinsic

        public event Action<Health> HealthChangeEvent; // Prev, new
        public event Action<Health> MaxHealthChangeEvent; // Prev, new

        public Health()
        {
            Current = max;
            RegenProgress = regenTime;
        }

        public Health(int max, int regenTime)
        {
            this.max = max;
            Current = this.max;
            this.regenTime = regenTime;
            RegenProgress = this.regenTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="change"></param>
        /// <returns>True if entity ran out of health.</returns>
        private bool ModifyHealth(int change)
        {
            int prev = Current;
            Current += change;
            HealthChangeEvent?.Invoke(this);
            if (Current <= 0)
                return true;
            else
                return false;
        }

        public void Heal(int healing)
        {
            // TODO: OnHealEvent
            ModifyHealth(healing);
        }

        public bool Damage(int damage)
        {
            // TODO: OnDamageEvent
            return ModifyHealth(-damage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the entity regenerated at all.</returns>
        public bool Regen()
        {
            // TODO: OnRegenEvent
            if (RegenTime == 0) // Does not regenerate
                return false;
            else
            {
                RegenProgress -= 100;
                if (RegenProgress <= 0)
                {
                    RegenProgress = RegenTime;
                    Heal(1);
                }
                return true;
            }
        }

        public override EntityComponent Clone() => new Health(max, regenTime);

        public override string ToString()
        {
            return $"Health: {Current}/{Max}, Regenerating: {Regenerating}";
        }
    }
}

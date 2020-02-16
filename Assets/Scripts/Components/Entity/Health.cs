// Health.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Utils;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class Health : EntityComponent
    {
        // TODO: Reduce values to int16 
        [SerializeField] private int max = 10;
        [SerializeField] private int regenTime = 800; // Time in ticks to regen 1 HP

        [JsonIgnore] public int Current { get; private set; }
        public int Max
        {
            get => max;
            set
            {
                max = value;
                MaxHealthChangeEvent?.Invoke(this);
            }
        }
        public int RegenTime { get => regenTime; set => regenTime = value; }
        [JsonIgnore] public int RegenProgress { get; set; } // Ticks down from regenTime until 0
        public bool Regenerating { get; set; } = true; // Status, not intrinsic
        public bool Invincible { get; set; } = false;

        public event Action<Health> HealthChangeEvent; // Prev, new
        public event Action<Health> MaxHealthChangeEvent; // Prev, new
        public event Action DamagedEvent;

        public Health()
        {
            Current = max;
            RegenProgress = regenTime;
            Locator.Scheduler.ClockTickEvent += Regen;
        }

        [JsonConstructor]
        public Health(int max, int regenTime)
        {
            this.max = max;
            Current = this.max;
            this.regenTime = regenTime;
            RegenProgress = this.regenTime;
            Locator.Scheduler.ClockTickEvent += Regen;
        }

        public Health(int max, int regenTime,
            int current, int regenProgress, bool regenerating, bool invincible)
            : this(max, regenTime)
        {
            Current = current;
            RegenTime = regenTime;
            RegenProgress = regenProgress;
            Regenerating = regenerating;
            Invincible = invincible;
        }

        /// <returns>True if entity ran out of health.</returns>
        private bool ModifyHealth(int change)
        {
            Current = Mathf.Clamp(Current + change, 0, Max);
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

        public bool Damage(Entity damager, HitDamage damage)
        {
            if (Invincible)
                return false;

            DamagedEvent?.Invoke();
            Message(new DamageEventMessage(this, damager));
            return ModifyHealth(-damage.amount);
        }

        public void Regen()
        {
            // TODO: OnRegenEvent
            if (RegenTime == 0 || !Regenerating)
                return;
            else
            {
                RegenProgress -= 100;
                if (RegenProgress <= 0)
                {
                    RegenProgress = RegenTime;
                    Heal(1);
                }
                return;
            }
        }

        public override EntityComponent Clone(bool full)
        {
            if (full)
                return new Health(Max, RegenTime, Current, RegenProgress, Regenerating, Invincible);
            else
                return new Health(Max, RegenTime);
        }

        public override string ToString()
        {
            return $"Health: {Current}/{Max}, Regenerating: {Regenerating}";
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctxt)
        {
            Helpers.ClearNonSerializableDelegates(ref HealthChangeEvent);
            Helpers.ClearNonSerializableDelegates(ref MaxHealthChangeEvent);
        }
    }
}

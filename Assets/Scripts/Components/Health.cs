// Health.cs
// Jerome Martina

using Newtonsoft.Json;
using UnityEngine;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Health : EntityComponent
    {
        [SerializeField] private int max = 10;
        [SerializeField] private int regenTime = 800; // Time in ticks to regen 1 HP

        [JsonIgnore] public int Current { get; set; }
        public int Max { get => max; set => max = value; }
        public int RegenTime { get => regenTime; set => regenTime = value; }
        [JsonIgnore] public int RegenProgress { get; set; } // Ticks down from regenTime until 0
        public bool Regenerating { get; set; } = true; // Status, not intrinsic

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

        public void ModifyHealth(int change)
        {
            Current += change;
        }

        public bool Regen()
        {
            if (RegenTime == 0) // Does not regenerate
                return false;
            else
            {
                RegenProgress -= 100;
                if (RegenProgress <= 0)
                {
                    RegenProgress = RegenTime;
                    ModifyHealth(1);
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

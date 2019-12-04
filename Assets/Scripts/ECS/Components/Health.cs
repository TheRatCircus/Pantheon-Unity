// Health.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Health : BaseComponent
    {
        [SerializeField] private int current;
        [SerializeField] private int max = -1;
        [SerializeField] private int regenRate = -1; // Time in ticks to regen 1 HP
        [SerializeField] private int regenProgress = 0;

        public int Current { get => current; set => current = value; }
        public int Max => max;
        public int RegenRate => regenRate;
        public int RegenProgress 
        {
            get => regenProgress; set => regenProgress = value;
        }

        public Health(int max, int regenRate)
        {
            this.max = max;
            current = this.max;
            this.regenRate = regenRate;
        }

        public override BaseComponent Clone()
        {
            return new Health(max, regenRate);
        }
    }
}

// Damage.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Definitional damage values for Melee, Ranged, etc.
    /// </summary>
    [System.Serializable]
    public sealed class Damage
    {
        [SerializeField] private DamageType type;
        [SerializeField] private int min = -1;
        [SerializeField] private int max = -1;

        public DamageType Type { get => type; set => type = value; }
        public int Min { get => min; set => min = value; }
        public int Max { get => max; set => max = value; }
    }

    /// <summary>
    /// Calculated damage values for hits.
    /// </summary>
    [System.Serializable]
    public struct HitDamage
    {
        public readonly DamageType type;
        public readonly int amount;

        public HitDamage(Damage damage)
        {
            type = damage.Type;
            amount = Random.Range(damage.Min, damage.Max + 1);
        }

        public HitDamage(DamageType type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}

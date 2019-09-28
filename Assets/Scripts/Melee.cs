// Melee.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Melee stats for items and parts, as well as cooldown status.
    /// </summary>
    [System.Serializable]
    public sealed class Melee
    {
        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;
        [SerializeField] private int accuracy = -1; // 0...100
        [SerializeField] private int attackTime = -1; // Lower is faster
        [SerializeField] private DamageType damageType = DamageType.None;

        // Properties
        public int MinDamage { get => minDamage; }
        public int MaxDamage { get => maxDamage; }
        public int Accuracy { get => accuracy; }
        public int AttackTime { get => attackTime; }
        public DamageType DamageType { get => damageType; }
    }

    public enum DamageType
    {
        None = 0,
        Slashing = 1,
        Piercing = 2,
        Bludgeoning = 3
    }
}

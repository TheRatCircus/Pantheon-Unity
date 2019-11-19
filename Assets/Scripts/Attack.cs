// Attack.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// An attack feasible by a Melee or Ranged component.
    /// </summary>
    [System.Serializable]
    public sealed class Attack
    {
        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;
        [SerializeField] private int accuracy = -1; // 0...100
        [SerializeField] private int time = -1;

        public int MinDamage => minDamage;
        public int MaxDamage => maxDamage;
        public int Accuracy => accuracy;
        public int Time => time;
    }
}

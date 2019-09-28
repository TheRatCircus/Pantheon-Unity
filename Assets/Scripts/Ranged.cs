// Ranged.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    [System.Serializable]
    public sealed class Ranged
    {
        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;
        [SerializeField] private int accuracy = -1;
        [SerializeField] private AmmoFamily ammoFamily;

        public int MinDamage
        { get => minDamage; private set => minDamage = value; }
        public int MaxDamage
        { get => maxDamage; private set => maxDamage = value; }
        public int Accuracy
        { get => accuracy; private set => accuracy = value; }
        public AmmoFamily AmmoFamily
        { get => ammoFamily; private set => ammoFamily = value; }
    }
}

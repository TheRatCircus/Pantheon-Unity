// Ammo.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    [System.Serializable]
    public sealed class Ammo
    {
        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;
        [SerializeField] private int accuracy = -1; // 0...100
        [SerializeField] private DamageType damageType = DamageType.None;
        [SerializeField] private bool pierces = false;
        [SerializeField] private GameObject fxPrefab;
        [SerializeField] private AmmoFamily ammoFamily = AmmoFamily.None;

        // Properties
        public int MinDamage { get => minDamage; }
        public int MaxDamage { get => maxDamage; }
        public int Accuracy { get => accuracy; }
        public DamageType DamageType { get => damageType; }
        public bool Pierces { get => pierces; }
        public GameObject FXPrefab { get => fxPrefab; private set => fxPrefab = value; }
        public AmmoFamily AmmoFamily { get => ammoFamily; }

        public override string ToString()
        {
            string min = $"minDamage: {minDamage}";
            string max = $"maxDamage: {maxDamage}";
            string acc = $"accuracy: {accuracy}";
            string dmgType = $"damageType: {damageType}";
            string p = $"pierces: {pierces}";
            string family = $"ammoFamily: {ammoFamily}";
            return $"{min} {max} {acc} {dmgType} {p} {family}";
        }
    }

    public enum AmmoFamily
    {
        None,
        Arrows,
        Bolts,
        Cartridges,
        Shot
    }
}


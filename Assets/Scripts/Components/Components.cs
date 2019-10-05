// Components.cs
// Jerome Martina

using System;
using UnityEngine;

namespace Pantheon.Components
{
    /// <summary>
    /// Used to prevent component duplicity.
    /// </summary>
    public enum ComponentType
    {
        Ranged,
        Ammo,
        Defenses,
        Flask,
        Scroll,
        Equipment,
        Corpse
    }

    public interface IComponent
    {
        ComponentType Type { get; }
    }

    /// <summary>
    /// Melee stats for items and appendages.
    /// </summary>
    [Serializable]
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

    [Serializable]
    public sealed class Ranged : IComponent
    {
        public ComponentType Type => ComponentType.Ranged;

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

    [Serializable]
    public sealed class Ammo : IComponent
    {
        public ComponentType Type => ComponentType.Ammo;

        [SerializeField] private string projName = "NO_AMMO_NAME";
        [SerializeField] private int minDamage = -1;
        [SerializeField] private int maxDamage = -1;
        [SerializeField] private int accuracy = -1; // 0...100
        [SerializeField] private DamageType damageType = DamageType.None;
        [SerializeField] private bool pierces = false;
        [SerializeField] private GameObject fxPrefab;
        [SerializeField] private AmmoFamily ammoFamily = AmmoFamily.None;

        // Properties
        public string ProjName { get => projName; }
        public int MinDamage { get => minDamage; }
        public int MaxDamage { get => maxDamage; }
        public int Accuracy { get => accuracy; }
        public DamageType DamageType { get => damageType; }
        public bool Pierces { get => pierces; }
        public GameObject FXPrefab
        {
            get => fxPrefab;
            private set => fxPrefab = value;
        }
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

    [Serializable]
    public sealed class Defenses : IComponent
    {
        public ComponentType Type => ComponentType.Defenses;

        [SerializeField] private int armour;
        [SerializeField] private int evasion;

        // Resists run from -1.0 to 1.0
        [SerializeField] private float resistPhys;
        [SerializeField] private float resistHeat;
        [SerializeField] private float resistCold;
        [SerializeField] private float resistDisease;
        [SerializeField] private float resistPoison;

        public int Armour { get => armour; set => armour = value; }
        public int Evasion { get => evasion; set => evasion = value; }
    }

    /// <summary>
    /// Component of any item that can be equipped e.g. armour, jewellery.
    /// </summary>
    [Serializable]
    public sealed class Equipment : IComponent
    {
        public ComponentType Type => ComponentType.Equipment;

        [SerializeField] private EquipType equipType = EquipType.None;

        public EquipType EquipType => equipType;
    }

    public enum EquipType
    {
        None,
        Head,
        Body,
        Shoulders,
        Gloves,
        Waist,
        Feet
    }

    [Serializable]
    public sealed class Corpse : IComponent
    {
        public ComponentType Type => ComponentType.Corpse;

        [SerializeField] private Species species;

        public Corpse(Actors.Actor actor) => species = actor.Species;
    }
}
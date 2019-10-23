// Appendage.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;

namespace Pantheon.Actors
{
    [System.Serializable]
    public class Appendage
    {
        [SerializeField]
        private AppendageDef appendageDef;

        // Properties for getting appendageDef
        public SpeciesID Species { get => appendageDef.Species; }
        public AppendageType Type { get => appendageDef.Type; }
        public string DisplayName { get => appendageDef.DisplayName; }
        public Sprite Sprite { get => appendageDef.Sprite; }
        public int MoveSpeed { get => appendageDef.MoveTime; }
        public bool Prehensile { get => appendageDef.Prehensile; }

        // Instance-specific
        [SerializeField] private bool dexterous; // Can melee, grasp if prehensile
        [SerializeField] private int strength;
        [SerializeField] private Melee melee;

        // Own properties
        public Item Item { get; set; } // Wielded item
        public int Strength { get => strength; set => strength = value; }
        public Melee Melee { get => melee; }
        public bool CanMelee { get => melee.MaxDamage > 0; }
        public bool Dexterous { get => dexterous; set => dexterous = value; }

        public Appendage(AppendageDef appendageDef)
        {
            this.appendageDef = appendageDef;

            strength = appendageDef.Strength;
            melee = appendageDef.Melee;

            if (appendageDef.InherentlyDexterous)
                dexterous = true;
        }

        public void Initialize()
        {
            /*
             * There's no way to have this object automatically pull its
             * base data when it loads, so this has to be called whenever
             * the actor which owns it Awakes
             */
            strength = appendageDef.Strength;
            melee = appendageDef.Melee;

            if (appendageDef.InherentlyDexterous)
                dexterous = true;
        }

        public override string ToString()
            => DisplayName;
    }
}

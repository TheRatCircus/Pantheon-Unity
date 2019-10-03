// BodyPart.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;

namespace Pantheon.Actors
{
    [System.Serializable]
    public class BodyPart
    {
        [SerializeField]
        private BodyPartData partData;

        // Properties for getting partData
        public SpeciesRef Species { get => partData.Species; }
        public BodyPartType Type { get => partData.Type; }
        public string DisplayName { get => partData.DisplayName; }
        public Sprite Sprite { get => partData.Sprite; }
        public int MoveSpeed { get => partData.MoveTime; }
        public bool Prehensile { get => partData.Prehensile; }

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

        public BodyPart(BodyPartData partData)
        {
            this.partData = partData;

            strength = partData.Strength;
            melee = partData.Melee;

            if (partData.InherentlyDexterous)
                dexterous = true;
        }

        public void Initialize()
        {
            /*
             * There's no way to have this object automatically pull its
             * base data when it loads, so this has to be called whenever
             * the actor which owns it Awakes
             */
            strength = partData.Strength;
            melee = partData.Melee;

            if (partData.InherentlyDexterous)
                dexterous = true;
        }

        public override string ToString()
            => DisplayName;
    }
}

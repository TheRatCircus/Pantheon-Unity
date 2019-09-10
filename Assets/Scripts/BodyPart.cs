// BodyPart.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Actors
{
    [System.Serializable]
    public class BodyPart
    {
        [SerializeField]
        private BodyPartData partData;

        // Properties for getting partData
        public Species Species { get => partData.Species; }
        public BodyPartType Type { get => partData.Type; }
        public string Name { get => partData.DisplayName; }
        public Sprite Sprite { get => partData.Sprite; }
        public int RunBonus { get => partData.RunBonus; }
        public bool Prehensile { get => partData.Prehensile; }

        // Instance-specific
        [SerializeField] private bool dominant;
        [SerializeField] private int strength;
        [SerializeField] private int dexterity;
        [SerializeField] private Melee melee;
        [SerializeField] private bool canMelee;

        // Own properties
        public Item Item { get; set; } // Wielded item
        public int Strength { get => strength; set => strength = value; }
        public Melee Melee { get => melee; }
        public bool CanMelee { get => canMelee; }

        public BodyPart(BodyPartData partData)
        {
            this.partData = partData;

            strength = partData.Strength;
            dexterity = partData.Dexterity;
            melee = partData.Melee;

            if (partData.InherentMelee)
                canMelee = true;
        }

        public void Initialize()
        {
            /*
             * There's no way to have this object automatically pull its
             * base data when it loads, so this has to be called whenever
             * the actor which owns it Awakes
             */
            strength = partData.Strength;
            dexterity = partData.Dexterity;
            melee = partData.Melee;

            if (partData.InherentMelee)
                canMelee = true;
        }
    }
}

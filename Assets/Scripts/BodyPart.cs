// BodyPart.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Actors
{
    /// <summary>
    /// An actualized body part.
    /// </summary>
    [System.Serializable]
    public class BodyPart
    {
        [SerializeField]
        private BodyPartData partData;

        // Properties for getting partData
        public Species Species { get => partData.species; }
        public BodyPartType Type { get => partData.type; }
        public string Name { get => partData.displayName; }
        public Sprite Sprite { get => partData.sprite; }

        public int RunBonus { get => partData.runBonus; }
        public bool Prehensile { get => partData.prehensile; }
        public bool CanMelee { get => partData.canMelee; }

        // Instance-specific
        public bool Dominant;
        public int Strength;
        public int Dexterity;

        public Melee melee;
        Item item; // Wielded item

        // Own properties
        public Item Item { get => item; set => item = value; }

        public BodyPart(BodyPartData partData)
        {
            this.partData = partData;

            Strength = partData.strength;
            Dexterity = partData.dexterity;
            melee = partData.melee;
        }
    }

}
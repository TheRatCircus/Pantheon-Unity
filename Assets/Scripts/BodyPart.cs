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
        public Species Species { get => partData.Species; }
        public BodyPartType Type { get => partData.Type; }
        public string Name { get => partData.DisplayName; }
        public Sprite Sprite { get => partData.Sprite; }

        public int RunBonus { get => partData.RunBonus; }
        public bool Prehensile { get => partData.Prehensile; }
        public bool CanMelee { get => partData.CanMelee; }

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

            Strength = partData.Strength;
            Dexterity = partData.Dexterity;
            melee = partData.Melee;
        }
    }

}
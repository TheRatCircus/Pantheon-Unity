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
        public bool CanMelee { get => partData.CanMelee; }

        // Instance-specific
        [SerializeField] private bool dominant;
        [SerializeField] private int strength;
        [SerializeField] private int dexterity;

        public Melee melee;

        // Own properties
        public Item Item { get; set; } // Wielded item
        public int Strength { get => strength; set => strength = value; }

        public BodyPart(BodyPartData partData)
        {
            this.partData = partData;

            Strength = partData.Strength;
            dexterity = partData.Dexterity;
            melee = partData.Melee;
        }
    }

}
// Equipment.cs
// Jerome Martina

namespace Pantheon.Actors
{
    /// <summary>
    /// Actor armour and accessory data.
    /// </summary>
    [System.Serializable]
    public sealed class Equipment
    {
        // Equipment
        public Item BodyWear { get; set; }
        public Item ShoulderWear { get; set; }
        public Item Amulet { get; set; }
        public Item Belt { get; set; }
        public Item Gloves { get; set; }
        public Item Boots { get; set; }

        public int GetArmour()
        {
            int armour = 0;

            if (BodyWear != null)
                armour += BodyWear.Defenses.Armour;
            if (ShoulderWear != null)
                armour += ShoulderWear.Defenses.Armour;
            if (Amulet != null)
                armour += Amulet.Defenses.Armour;
            if (Belt != null)
                armour += Belt.Defenses.Armour;
            if (Gloves != null)
                armour += Gloves.Defenses.Armour;
            if (Boots != null)
                armour += Boots.Defenses.Armour;

            return armour;
        }
    }
}
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
        public Item HeadWear { get; set; }
        public Item BodyWear { get; set; }
        public Item ShoulderWear { get; set; }
        public Item Amulet { get; set; }
        public Item Belt { get; set; }
        public Item Handwear { get; set; }
        public Item Footwear { get; set; }

        public int GetArmour()
        {
            int armour = 0;

            if (HeadWear != null)
                armour += HeadWear.GetComponent<Defenses>().Armour;
            if (BodyWear != null)
                armour += BodyWear.GetComponent<Defenses>().Armour;
            if (ShoulderWear != null)
                armour += ShoulderWear.GetComponent<Defenses>().Armour;
            if (Amulet != null)
                armour += Amulet.GetComponent<Defenses>().Armour;
            if (Belt != null)
                armour += Belt.GetComponent<Defenses>().Armour;
            if (Handwear != null)
                armour += Handwear.GetComponent<Defenses>().Armour;
            if (Footwear != null)
                armour += Footwear.GetComponent<Defenses>().Armour;

            return armour;
        }
    }
}
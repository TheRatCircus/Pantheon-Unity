// Weights.cs
// Jerome Martina

using static Pantheon.Utils.RandomUtils;

namespace Pantheon.WorldGen
{
    public sealed class Weights
    {
        public static GenericRandomPick<ItemCategory>[] _itemCategories =
        {
            new GenericRandomPick<ItemCategory>(400, ItemCategory.Weapon),
            new GenericRandomPick<ItemCategory>(32, ItemCategory.Ammo),
            new GenericRandomPick<ItemCategory>(256, ItemCategory.Wearable),
            new GenericRandomPick<ItemCategory>(512, ItemCategory.Consumable),
            new GenericRandomPick<ItemCategory>(64, ItemCategory.Misc)
        };

        public static GenericRandomPick<ItemID>[] _weapons = 
        {
            new GenericRandomPick<ItemID>(512, ItemID.Hatchet)
        };

        public static GenericRandomPick<ItemID>[] _ammos =
        {
            new GenericRandomPick<ItemID>(512, ItemID.Cartridges)
        };

        public static GenericRandomPick<ItemID>[] _wearables =
        {
            new GenericRandomPick<ItemID>(512, ItemID.Cuirass)
        };

        public static GenericRandomPick<ItemID>[] _consumables = 
        {
            new GenericRandomPick<ItemID>(512, ItemID.Flask)
        };
    }
}
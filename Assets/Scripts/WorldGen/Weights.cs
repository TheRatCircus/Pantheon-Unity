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

        public static GenericRandomPick<string>[] _weapons = 
        {
            new GenericRandomPick<string>(512, ID.Item._hatchet)
        };

        public static GenericRandomPick<string>[] _ammos =
        {
            new GenericRandomPick<string>(512, ID.Item._cartridges)
        };

        public static GenericRandomPick<string>[] _wearables =
        {
            new GenericRandomPick<string>(512, ID.Item._cuirass)
        };

        public static GenericRandomPick<string>[] _consumables = 
        {
            new GenericRandomPick<string>(512, ID.Item._laudanum)
        };
    }
}

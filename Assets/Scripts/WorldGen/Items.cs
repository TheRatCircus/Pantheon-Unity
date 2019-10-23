// Items.cs
// Jerome Martina

using Pantheon.World;
using static Pantheon.Utils.RandomUtils;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for distributing items in a level.
    /// </summary>
    public static class Items
    {
        /// <summary>
        /// Spawn items at random throughout the level.
        /// </summary>
        public static void SpawnItems(Level level)
        {
            // 30 items in the level
            for (int i = 0; i < 30; i++)
            {
                Item item;
                ItemID itemID;

                // weighted roll between weapon, wearable, consumable, misc
                ItemCategory category = Weights._itemCategories.RandomPick(true);
                switch (category)
                {
                    case ItemCategory.Weapon:
                        itemID = Weights._weapons.RandomPick(true);
                        break;
                    case ItemCategory.Ammo:
                        itemID = Weights._ammos.RandomPick(true);
                        break;
                    case ItemCategory.Wearable:
                        itemID = Weights._wearables.RandomPick(true);
                        break;
                    case ItemCategory.Consumable:
                        itemID = Weights._consumables.RandomPick(true);
                        break;
                    case ItemCategory.Misc:
                        // TODO: Actual category of misc items
                        itemID = Weights._consumables.RandomPick(true);
                        break;
                    default:
                        throw new System.Exception("Illegal item category.");
                }

                item = Item.NewItem(itemID);

                if (category == ItemCategory.Weapon || 
                    category == ItemCategory.Wearable)
                {
                    if (Core.Game.PRNG.Next(4) == 3)
                        Enchant.EnchantItem(item);
                }

                level.RandomFloor().Items.Add(item);
            }
        }
    }
}

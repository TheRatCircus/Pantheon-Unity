// LevelItems.cs
// Jerome Martina

using Pantheon.World;
using static Pantheon.Utils.ItemFactory;
using static ItemWeights;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for distributing items in a level.
    /// </summary>
    public static class LevelItems
    {
        /// <summary>
        /// Spawn items at random throughout the level.
        /// </summary>
        public static void SpawnItems(ref Level level)
        {
            SpawnFlasks(ref level);
            SpawnScrolls(ref level);
            level.RandomFloor().Items.Add(NewWeapon(WeaponType.Hatchet));
            level.RandomFloor().Items.Add(NewWeapon(WeaponType.Prejudice));
            for (int i = 0; i < 2; i++)
                level.RandomFloor().Items.Add(NewWeapon(WeaponType.Dagger));
        }

        public static void SpawnFlasks(ref Level level)
        {
            for (int i = 0; i < 10; i++)
            {
                Item item = NewFlask(RandomWeighted(FlaskWeights));
                level.RandomFloor().Items.Add(item);
            }
        }

        public static void SpawnScrolls(ref Level level)
        {
            for (int i = 0; i < 10; i++)
            {
                Item item = NewScroll(RandomWeighted(ScrollWeights));
                level.RandomFloor().Items.Add(item);
            }
        }
    }
}
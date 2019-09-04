// LevelItems.cs
// Jerome Martina

using static Pantheon.Utils.RandomUtils;

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
        }

        public static void SpawnFlasks(ref Level level)
        {
            for (int i = 0; i < 10; i++)
            {
                // TODO: This is verbose
                Item item = new Item(
                    Database.GetFlask
                    (ItemWeights.FlaskWeights[RandomPick
                    (ItemWeights.FlaskWeights)].Value));
                level.RandomFloor().Items.Add(item);
            }
        }

        public static void SpawnScrolls(ref Level level)
        {
            for (int i = 0; i < 10; i++)
            {
                Item item = new Item(
                    Database.GetScroll
                    (ItemWeights.ScrollWeights[RandomPick
                    (ItemWeights.ScrollWeights)].Value));
                level.RandomFloor().Items.Add(item);
            }
        }
    }
}
// LevelZones.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for generating levels by their zones.
    /// </summary>
    public static class LevelZones
    {
        public const int ValleySize = 64;
        public const int ValleyEnemies = 10;

        /// <summary>
        /// Generate a level from the Valley zone.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="section">Whether to generate Northern, Eastern, etc.</param>
        public static void GenerateValley(ref Level level, CardinalDirection section)
        {
            level.DisplayName = "Central Valley";
            level.LevelSize = new Vector2Int(ValleySize, ValleySize);

            Debug.Log($"Initializing cells for {level.DisplayName}...");
            level.Map = LevelLayout.BlankMap(level.LevelSize);

            LevelLayout.RandomFill(ref level, 15, TerrainType.StoneWall);
            LevelLayout.Enclose(ref level, TerrainType.StoneWall);

            // If generating the Central Valley, then spawn the player there
            if (section == CardinalDirection.Centre)
            {
                Debug.Log("Spawning the player...");
                level.SpawnPlayer();
            }

            LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyPop);
            LevelItems.SpawnItems(ref level);

            CellDrawer.DrawLevel(level);
        }
    }
}
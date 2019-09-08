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
        /// <param name="wing">Whether to generate Northern, Eastern, etc.</param>
        public static void GenerateValley(ref Level level, CardinalDirection wing)
        {
            level.LevelSize = new Vector2Int(ValleySize, ValleySize);

            Debug.Log($"Initializing cells...");
            level.Map = LevelLayout.BlankMap(level.LevelSize);

            LevelLayout.RandomFill(ref level, 15, TerrainType.StoneWall);
            LevelLayout.Enclose(ref level, TerrainType.StoneWall);

            // If generating the Central Valley, then spawn the player there
            if (wing == CardinalDirection.Centre)
            {
                Debug.Log("Spawning the player...");
                level.SpawnPlayer();
            }

            // Generate display name and connections
            switch (wing)
            {
                case CardinalDirection.Centre:
                    level.DisplayName = "Central Valley";
                    Cell connectionCell = level.RandomFloor();
                    connectionCell.Connection = new LateralConnection(
                        level,
                        connectionCell,
                        Database.GetFeature(FeatureType.StairsDown),
                        GenerateValley, CardinalDirection.North);
                    break;
                case CardinalDirection.North:
                    level.DisplayName = "Northern Valley";
                    break;
                default:
                    Debug.LogException(new System.Exception("No section passed to GenerateValley()."));
                    return;
            }

            //LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyPop);
            LevelItems.SpawnItems(ref level);

            CellDrawer.DrawLevel(level);
        }
    }
}
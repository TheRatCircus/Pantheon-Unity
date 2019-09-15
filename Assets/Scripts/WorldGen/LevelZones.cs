// LevelZones.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;

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

            UnityEngine.Debug.Log($"Initializing cells...");
            level.Map = LevelLayout.BlankMap(level.LevelSize, TerrainType.Grass);

            if (wing == CardinalDirection.Centre)
                LevelLayout.RandomFill(ref level, 15, FeatureType.Tree);

            LevelLayout.Enclose(ref level, TerrainType.StoneWall);

            // If generating the Central Valley, then spawn the player there
            if (wing == CardinalDirection.Centre)
            {
                UnityEngine.Debug.Log("Spawning the player...");
                level.SpawnPlayer();
            }

            UnityEngine.Debug.Log("Generating wing specifics for new valley level...");
            GenerateValleyWing(ref level, wing);
            LevelItems.SpawnItems(ref level);

            UnityEngine.Debug.Log($"Registering level {level.RefName} in dictionary...");
            Game.instance.RegisterLevel(level);
            CellDrawer.DrawLevel(level);
        }

        /// <summary>
        /// Generate wing specifics for a valley zone.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="wing">Whether to generate Northern, Eastern, etc.</param>
        public static void GenerateValleyWing(ref Level level, CardinalDirection wing)
        {
            switch (wing)
            {
                case CardinalDirection.Centre:
                    {
                        level.DisplayName = "Central Valley";
                        level.RefName = "valleyCentre";

                        level.Connections = new Dictionary<string, Connection>(4);

                        Connection trailNorth = LevelConnections.MapEdgeConnection(
                            level, CardinalDirection.North,
                            FeatureType.TrailNorth, GenerateValley);
                        trailNorth.DisplayName = "a trail to the Northern Valley";
                        level.Connections.Add("trailNorth", trailNorth);

                        Connection trailEast = LevelConnections.MapEdgeConnection(
                            level, CardinalDirection.East,
                            FeatureType.TrailEast, GenerateValley);
                        trailEast.DisplayName = "a trail to the Eastern Valley";
                        level.Connections.Add("trailEast", trailEast);

                        Connection trailSouth = LevelConnections.MapEdgeConnection(
                            level, CardinalDirection.South,
                            FeatureType.TrailSouth, GenerateValley);
                        trailSouth.DisplayName = "a trail to the Southern Valley";
                        level.Connections.Add("trailSouth", trailSouth);

                        Connection trailWest = LevelConnections.MapEdgeConnection(
                            level, CardinalDirection.West,
                            FeatureType.TrailWest, GenerateValley);
                        trailWest.DisplayName = "a trail to the Western Valley";
                        level.Connections.Add("trailWest", trailWest);

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyCentre);

                        break;
                    }
                case CardinalDirection.North:
                    {
                        level.DisplayName = "Northern Valley";
                        level.RefName = "valleyNorth";

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyNorth);

                        level.Connections = new Dictionary<string, Connection>(1);

                        Cell trailSouthCell = level.RandomFloor(-1, 1);
                        Connection trailSouth = LevelConnections.ConnectZones(
                            level, trailSouthCell, "valleyCentre", "trailNorth",
                            FeatureType.TrailSouth);
                        trailSouth.DisplayName = "a trail to the Central Valley";

                        level.Connections.Add("trailSouth", trailSouth);

                        break;
                    }
                case CardinalDirection.East:
                    {
                        level.DisplayName = "Eastern Valley";
                        level.RefName = "valleyEast";

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyNorth);

                        level.Connections = new Dictionary<string, Connection>(1);

                        Cell trailWestCell = level.RandomFloor(1, -1);
                        Connection trailWest = LevelConnections.ConnectZones(
                            level, trailWestCell, "valleyCentre", "trailEast",
                            FeatureType.TrailWest);
                        trailWest.DisplayName = "a trail to the Central Valley";

                        level.Connections.Add("trailWest", trailWest);

                        break;
                    }
                case CardinalDirection.South:
                    {
                        level.DisplayName = "Southern Valley";
                        level.RefName = "valleySouth";

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyNorth);

                        level.Connections = new Dictionary<string, Connection>(1);

                        Cell trailNorthCell = level.RandomFloor(-1, level.LevelSize.y - 2);
                        Connection trailNorth = LevelConnections.ConnectZones(
                            level, trailNorthCell, "valleyCentre", "trailSouth",
                            FeatureType.TrailNorth);
                        trailNorth.DisplayName = "a trail to the Central Valley";

                        level.Connections.Add("trailNorth", trailNorth);

                        break;
                    }
                case CardinalDirection.West:
                    {
                        level.DisplayName = "Western Valley";
                        level.RefName = "valleyWest";

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyNorth);

                        level.Connections = new Dictionary<string, Connection>(1);

                        Cell trailEastCell = level.RandomFloor(level.LevelSize.x - 2, -1);
                        Connection trailEast = LevelConnections.ConnectZones(
                            level, trailEastCell, "valleyCentre", "trailWest",
                            FeatureType.TrailEast);
                        trailEast.DisplayName = "a trail to the Central Valley";

                        level.Connections.Add("trailEast", trailEast);

                        break;
                    }
                default:
                    throw new System.Exception
                        ("No wing passed to GenerateValleyConnections().");
            }
        }

        public static void GenerateDomainAtrium(Level level)
        {
            level.DisplayName = "IDOL_NAME's Domain: Atrium";
            level.RefName = "IDOL_NAMEAtrium";

            level.LevelSize = new Vector2Int(128, 128);
            level.Connections = new Dictionary<string, Connection>(1);

            level.Map = LevelLayout.BlankMap(level.LevelSize, TerrainType.StoneWall);
            BinarySpacePartition.BSP(level, TerrainType.MarbleTile, 12);

            Cell stairsUpCell = level.RandomFloor();
            Connection stairsUp = new VerticalConnection(
                level, stairsUpCell,
                Database.GetFeature(FeatureType.StairsUp),
                GenerateDomainLevel, 2);
            stairsUpCell.Connection = stairsUp;
             
            stairsUp.DisplayName = "a flight of stairs to IDOL_NAME's Centrum";
            level.Connections.Add("stairsUp", stairsUp);

            Game.instance.RegisterLevel(level);
            CellDrawer.DrawLevel(level);
        }

        public static void GenerateDomainLevel(Level level, int floor)
        {
            level.LevelSize = new Vector2Int(128, 128);
            if (floor == 2)
            {
                level.DisplayName = "IDOL_NAME's Domain: Centrum";
                level.RefName = "IDOL_NAMECentrum";

                level.Connections = new Dictionary<string, Connection>(2);

                level.Map = LevelLayout.BlankMap(level.LevelSize, TerrainType.StoneWall);
                BinarySpacePartition.BSP(level, TerrainType.MarbleTile, 12);

                Cell stairsDownCell = level.RandomFloor();
                Connection stairsDown = LevelConnections.ConnectZones(
                    level, stairsDownCell, "IDOL_NAMEAtrium", "stairsUp",
                    FeatureType.StairsDown);
                stairsDownCell.Connection = stairsDown;
                stairsDown.DisplayName = "a flight of stairs to IDOL_NAME's Atrium";
                level.Connections.Add("stairsDown", stairsDown);

                Cell stairsUpCell = level.RandomFloor();
                Connection stairsUp = new VerticalConnection(
                    level, stairsUpCell,
                    Database.GetFeature(FeatureType.StairsUp),
                    GenerateDomainLevel, 3);
                stairsUpCell.Connection = stairsUp;
                stairsUp.DisplayName = "a flight of stairs to IDOL_NAME's Sanctorum";
                level.Connections.Add("stairsUp", stairsUp);
            }
            else if (floor == 3)
            {
                level.DisplayName = "IDOL_NAME's Sanctorum";
                level.RefName = "IDOL_NAMESanctorum";

                level.Connections = new Dictionary<string, Connection>(1);

                level.Map = LevelLayout.BlankMap(level.LevelSize, TerrainType.StoneWall);
                BinarySpacePartition.BSP(level, TerrainType.MarbleTile, 12);

                Cell stairsDownCell = level.RandomFloor();
                Connection stairsDown = LevelConnections.ConnectZones(
                    level, stairsDownCell, "IDOL_NAMECentrum", "stairsUp",
                    FeatureType.StairsDown);
                stairsDownCell.Connection = stairsDown;
                stairsDown.DisplayName = "a flight of stairs to IDOL_NAME's Centrum";
                level.Connections.Add("stairsDown", stairsDown);
            }
            else throw new System.ArgumentException
                ("Floor can only be 2 or 3.");
        }
    }
}
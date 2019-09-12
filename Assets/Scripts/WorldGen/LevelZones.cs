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

            Debug.Log($"Initializing cells...");
            level.Map = LevelLayout.BlankMap(level.LevelSize, TerrainType.Grass);

            if (wing == CardinalDirection.Centre)
                LevelLayout.RandomFill(ref level, 15, FeatureType.Tree);

            LevelLayout.Enclose(ref level, TerrainType.StoneWall);

            // If generating the Central Valley, then spawn the player there
            if (wing == CardinalDirection.Centre)
            {
                Debug.Log("Spawning the player...");
                level.SpawnPlayer();
            }

            GenerateValleyWing(ref level, wing);
            LevelItems.SpawnItems(ref level);

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

                        Cell trailNorthCell = level.RandomFloor(-1, level.LevelSize.y - 2);
                        Connection trailNorth = new LateralConnection(
                            level, trailNorthCell,
                            Database.GetFeature(FeatureType.TrailNorth),
                            GenerateValley, CardinalDirection.North);
                        trailNorthCell.Connection = trailNorth;
                        level.Connections.Add("trailNorth", trailNorth);

                        Cell trailEastCell = level.RandomFloor(level.LevelSize.x - 2, -1);
                        Connection trailEast = new LateralConnection(
                            level, trailEastCell,
                            Database.GetFeature(FeatureType.TrailEast),
                            GenerateValley, CardinalDirection.East);
                        trailEastCell.Connection = trailEast;
                        level.Connections.Add("trailEast", trailEast);

                        Cell trailSouthCell = level.RandomFloor(-1, 1);
                        Connection trailSouth = new LateralConnection(
                            level, trailSouthCell,
                            Database.GetFeature(FeatureType.TrailSouth),
                            GenerateValley, CardinalDirection.South);
                        trailSouthCell.Connection = trailSouth;
                        level.Connections.Add("trailSouth", trailSouth);

                        Cell trailWestCell = level.RandomFloor(1, -1);
                        Connection trailWest = new LateralConnection(
                            level, trailWestCell,
                            Database.GetFeature(FeatureType.TrailWest),
                            GenerateValley, CardinalDirection.West);
                        trailWestCell.Connection = trailWest;
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

                        if (!Game.instance.levels.TryGetValue("valleyCentre", out Level centralValley))
                            throw new System.Exception
                                ("Central valley was not generated, or has bad ref.");

                        if (!centralValley.Connections.TryGetValue("trailNorth", out Connection trailNorth))
                            throw new System.Exception("Central Valley has no trail north.");

                        Cell trailSouthCell = level.RandomFloor(-1, 1);
                        Connection trailSouth = new LateralConnection(
                            level, trailSouthCell,
                            Database.GetFeature(FeatureType.TrailSouth),
                            trailNorth);
                        trailSouthCell.Connection = trailSouth;
                        level.Connections.Add("trailSouth", trailSouth);

                        break;
                    }
                case CardinalDirection.East:
                    {
                        level.DisplayName = "Eastern Valley";
                        level.RefName = "valleyEast";

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyNorth);

                        level.Connections = new Dictionary<string, Connection>(1);

                        if (!Game.instance.levels.TryGetValue("valleyCentre", out Level centralValley))
                            throw new System.Exception
                                ("Central valley was not generated, or has bad ref.");

                        if (!centralValley.Connections.TryGetValue("trailEast", out Connection trailEast))
                            throw new System.Exception("Central Valley has no trail east.");

                        Cell trailWestCell = level.RandomFloor(1, -1);
                        Connection trailWest = new LateralConnection(
                            level, trailWestCell,
                            Database.GetFeature(FeatureType.TrailWest),
                            trailEast);
                        trailWestCell.Connection = trailWest;
                        level.Connections.Add("trailWest", trailWest);

                        break;
                    }
                case CardinalDirection.South:
                    {
                        level.DisplayName = "Southern Valley";
                        level.RefName = "valleySouth";

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyNorth);

                        level.Connections = new Dictionary<string, Connection>(1);

                        if (!Game.instance.levels.TryGetValue("valleyCentre", out Level centralValley))
                            throw new System.Exception
                                ("Central valley was not generated, or has bad ref.");

                        if (!centralValley.Connections.TryGetValue("trailSouth", out Connection trailSouth))
                            throw new System.Exception("Central Valley has no trail south.");

                        Cell trailNorthCell = level.RandomFloor(-1, level.LevelSize.y - 2);
                        Connection trailNorth = new LateralConnection(
                            level, trailNorthCell,
                            Database.GetFeature(FeatureType.TrailNorth),
                            trailSouth);
                        trailNorthCell.Connection = trailNorth;
                        level.Connections.Add("trailNorth", trailNorth);

                        break;
                    }
                case CardinalDirection.West:
                    {
                        level.DisplayName = "Western Valley";
                        level.RefName = "valleyWest";

                        LevelEnemies.SpawnNPCs(ref level, ValleyEnemies, NPCPops.ValleyNorth);

                        level.Connections = new Dictionary<string, Connection>(1);

                        if (!Game.instance.levels.TryGetValue("valleyCentre", out Level centralValley))
                            throw new System.Exception
                                ("Central valley was not generated, or has bad ref.");

                        if (!centralValley.Connections.TryGetValue("trailWest", out Connection trailWest))
                            throw new System.Exception("Central Valley has no trail west.");

                        Cell trailEastCell = level.RandomFloor(level.LevelSize.x - 2, -1);
                        Connection trailEast = new LateralConnection(
                            level, trailEastCell,
                            Database.GetFeature(FeatureType.TrailEast),
                            trailWest);
                        trailEastCell.Connection = trailEast;
                        level.Connections.Add("trailEast", trailEast);

                        break;
                    }
                default:
                    throw new System.Exception
                        ("No wing passed to GenerateValleyConnections().");
            }
        }
    }
}
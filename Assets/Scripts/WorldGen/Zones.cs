// Zones.cs
// Jerome Martina

using System;
using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;

namespace Pantheon.WorldGen
{
    public struct LevelGenArgs
    {
        public Vector3Int Coords { get; }
        public Idol Idol { get; }

        public LevelGenArgs(Vector3Int coords, Idol idol)
        {
            Coords = coords;
            Idol = idol;
        }

        public static LevelGenArgs ArgsFromRef(string levelRef)
        {
            string[] tokens = levelRef.Split('_');
            if (tokens[0] == "domain")
            {
                if (!Game.instance.Pantheon.Idols.TryGetValue(tokens[1],
                    out Idol idol))
                    throw new Exception("levelRef has an illegal idol in it.");

                int domainLevel = int.Parse(tokens[2]);

                return new LevelGenArgs(new Vector3Int(0, domainLevel, 0), idol);
            }
            else
                throw new ArgumentException("Arguments incomplete or invalid.");
        }
    }

    /// <summary>
    /// Functions for generating levels by their zones.
    /// </summary>
    public static class Zones
    {
        public const int ValleySize = 64;
        public const int ValleyEnemies = 10;

        public static void FinishLevel(Level level)
        {
            UnityEngine.Debug.Log($"Registering level {level.RefName} in dictionary...");
            Game.instance.RegisterLevel(level);
            CellDrawer.DrawLevel(level);
        }

        public static void ValleyBasics(Level level)
        {
            level.LevelSize = new Vector2Int(ValleySize, ValleySize);

            UnityEngine.Debug.Log($"Initializing cells...");
            level.Map = Layout.BlankMap(level.LevelSize, TerrainType.Grass);
        }

        public static void Valley(Level level, LevelGenArgs args)
        {
            if (!Game.instance.Layers.TryGetValue(args.Coords.z, out Layer layer))
                throw new Exception("Z-coord argument has no layer.");

            level.Layer = layer;
            ValleyBasics(level);

            Layout.Rectangle rect = new Layout.Rectangle(
                new Vector2Int(0, 0),
                new Vector2Int(level.LevelSize.x, level.LevelSize.y));

            Layout.FillRect(level, rect, FeatureType.WoodFence);
            BinarySpacePartition.BSP(level, TerrainType.Grass, 12);
            Layout.RandomFill(level, 7, FeatureType.Tree);
            Layout.Enclose(level, TerrainType.StoneWall);

            Vector2Int layerPos = Helpers.V3IToV2I(args.Coords);
            level.LayerPos = layerPos;
            CardinalDirection wing = Helpers.V2IToCardinal(layerPos);

            switch (wing)
            {
                case CardinalDirection.Centre:
                    {
                        level.DisplayName = "Central Valley";
                        level.RefName = "valley_central";

                        Connect.Trails(level, CardinalDirection.North
                            | CardinalDirection.East | CardinalDirection.South
                            | CardinalDirection.West);

                        break;
                    }
                case CardinalDirection.North:
                    {
                        level.DisplayName = "Northern Valley";
                        level.RefName = "valley_north";

                        PlaceGuaranteedAltar(level.RandomFloor());
                        Spawn.SpawnNPC(
                            Database.GetNPC(NPCType.DreadHamster).Prefab,
                            level, level.RandomFloor());

                        Connect.Trails(level, CardinalDirection.South);
                    }
                    break;
                case CardinalDirection.East:
                    {
                        level.DisplayName = "Eastern Valley";
                        level.RefName = "valley_east";

                        PlaceGuaranteedAltar(level.RandomFloor());

                        Connect.Trails(level, CardinalDirection.West);
                    }
                    break;
                case CardinalDirection.South:
                    {
                        level.DisplayName = "Southern Valley";
                        level.RefName = "valley_south";

                        PlaceGuaranteedAltar(level.RandomFloor());

                        Connect.Trails(level, CardinalDirection.North);
                    }
                    break;
                case CardinalDirection.West:
                    {
                        level.DisplayName = "Western Valley";
                        level.RefName = "valley_west";

                        PlaceGuaranteedAltar(level.RandomFloor());

                        Connect.Trails(level, CardinalDirection.East);
                    }
                    break;
                default:
                    throw new ArgumentException("Unhandled wing passed as coord.");
            }
            UnityEngine.Debug.Log($"Generating {level.RefName}...");

            Items.SpawnItems(level);
            NPCs.SpawnNPCs(level, ValleyEnemies, NPCPops.ValleyCentre);

            // Defer player spawn so RefreshFOV() covers everything
            if (wing == CardinalDirection.Centre)
            {
                UnityEngine.Debug.Log("Spawning the player...");
                Game.instance.LoadLevel(level);
                level.SpawnPlayer();
            }

            FinishLevel(level);
        }

        public static void PlaceGuaranteedAltar(Cell cell)
        {
            foreach (Idol idol in Game.instance.Pantheon.Idols.Values)
                if (!idol.HasAnAltar)
                {
                    cell.SetAltar(new Altar(idol, idol.Aspect.AltarFeature));
                    return;
                }
        }

        public static void Domain(Level level, LevelGenArgs args)
        {
            if (args.Idol == null)
                throw new ArgumentException("Domain generation needs an idol.");

            level.LevelSize = new Vector2Int(100, 100);

            level.DisplayName = "";
            level.RefName = $"domain_{args.Idol.RefName}_{args.Coords.y}";
            level.Faction = args.Idol.Religion;

            UnityEngine.Debug.Log($"Initializing cells...");
            level.Map = Layout.BlankMap(level.LevelSize, TerrainType.StoneWall);
            BinarySpacePartition.BSP(level, TerrainType.MarbleTile, 12);

            switch (args.Coords.y)
            {
                case 0:
                    {
                        Connection stairsUp = new Connection(level, level.RandomFloor(),
                        FeatureType.StairsUp,
                        $"domain_{args.Idol.RefName}_1");

                        level.UpConnections = new Connection[]
                        {
                            stairsUp
                        };
                        break;
                    }
                case 1:
                    {
                        Connection stairsDown = new Connection(level, level.RandomFloor(),
                            FeatureType.StairsDown,
                            $"domain_{args.Idol.RefName}_0");
                        Connection stairsUp = new Connection(level, level.RandomFloor(),
                            FeatureType.StairsUp,
                            $"domain_{args.Idol.RefName}_2");

                        level.DownConnections = new Connection[]
                        {
                            stairsDown
                        };
                        level.UpConnections = new Connection[]
                        {
                            stairsUp
                        };
                        break;
                    }
                case 2:
                    {
                        Connection stairsDown = new Connection(level, level.RandomFloor(),
                            FeatureType.StairsDown,
                            $"domain_{args.Idol.RefName}_1");

                        level.DownConnections = new Connection[]
                        {
                            stairsDown
                        };

                        Spawn.SpawnIdol(args.Idol, level, level.RandomFloor());

                        break;
                    }
                default:
                    throw new ArgumentException("Domain depth is out of range.");
            }
            FinishLevel(level);
        }
    }
}
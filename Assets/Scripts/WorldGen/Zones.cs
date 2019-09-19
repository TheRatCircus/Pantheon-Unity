// Zones.cs
// Jerome Martina

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
            UnityEngine.Debug.Log(args.Coords);

            if (!Game.instance.Layers.TryGetValue(args.Coords.z, out Layer layer))
                throw new System.Exception("Z-coord argument has no layer.");

            level.Layer = layer;
            ValleyBasics(level);
            
            Layout.RandomFill(level, 15, FeatureType.Tree);
            Layout.Enclose(level, TerrainType.StoneWall);

            Vector2Int layerPos = Helpers.V3IToV2I(args.Coords);
            level.LayerPos = layerPos;
            CardinalDirection wing = Helpers.V2IToCardinal(layerPos);

            switch (wing)
            {
                case CardinalDirection.Centre:
                    {
                        level.DisplayName = "Central Valley";
                        level.LevelRef = new LevelRef("valleyCentral");

                        Connect.Trails(level,
                            CardinalDirection.North
                            | CardinalDirection.East
                            | CardinalDirection.South
                            | CardinalDirection.West);

                        break;
                    }
                case CardinalDirection.North:
                    {
                        level.DisplayName = "Northern Valley";
                        level.LevelRef = new LevelRef("valleyNorth");

                        Connect.Trails(level,
                            CardinalDirection.South);
                    }
                    break;
                case CardinalDirection.East:
                    {
                        level.DisplayName = "Eastern Valley";
                        level.LevelRef = new LevelRef("valleyEast");

                        Connect.Trails(level,
                            CardinalDirection.West);
                    }
                    break;
                case CardinalDirection.South:
                    {
                        level.DisplayName = "Southern Valley";
                        level.LevelRef = new LevelRef("valleySouth");

                        Connect.Trails(level,
                            CardinalDirection.North);
                    }
                    break;
                case CardinalDirection.West:
                    {
                        level.DisplayName = "Western Valley";
                        level.LevelRef = new LevelRef("valleyWest");

                        Connect.Trails(level,
                            CardinalDirection.East);
                    }
                    break;
                default:
                    throw new System.ArgumentException("Unhandled wing passed as coord.");
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

        public static void Domain(Level level, LevelGenArgs args)
        {
            if (args.Idol == null)
                throw new System.ArgumentException("Domain generation needs an idol.");

            level.LevelSize = new Vector2Int(100, 100);

            UnityEngine.Debug.Log($"Initializing cells...");
            level.Map = Layout.BlankMap(level.LevelSize, TerrainType.MarbleTile);
            BinarySpacePartition.BSP(level, TerrainType.StoneWall, 16);

            FinishLevel(level);
        }
    }
}
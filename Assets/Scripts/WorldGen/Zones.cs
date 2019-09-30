// Zones.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.World;
using UnityEngine;
using static Pantheon.WorldGen.Layout;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for generating levels by their zones.
    /// </summary>
    public static class Zones
    {
        public const int ValleySize = 80;
        public const int ValleyEnemies = 20;

        public static void FinishLevel(Level level)
        {
            UnityEngine.Debug.Log
                ($"Registering level {level.RefName} in dictionary...");
            Game.instance.RegisterLevel(level);
            CellDrawer.DrawLevel(level);
        }

        public static void ValleyBasics(Level level)
        {
            level.LevelSize = new Vector2Int(ValleySize, ValleySize);
            level.Map = BlankMap(level.LevelSize, TerrainType.Grass);
        }

        public static void GenerateOuterValley(Level level)
        {
            ValleyBasics(level);

            int r = Utils.RandomUtils.RangeInclusive(0, 3);

            switch (r)
            {
                case 0: // Abandoned pastures
                    {
                        LevelRect rect = new LevelRect(new Vector2Int(0, 0),
                            new Vector2Int(
                                level.LevelSize.x,
                                level.LevelSize.y));

                        FillRect(level, rect, FeatureType.WoodFence);
                        BinarySpacePartition.BSP(level, TerrainType.Grass, 12);
                        Enclose(level, TerrainType.StoneWall);
                        foreach (Cell c in level.Map)
                            if (Utils.RandomUtils.OneChanceIn(6, true))
                                c.SetFeature(FeatureType.None); // Ruin fence
                        break;
                    }
                case 1: // Sparse wood
                    RandomFill(level, 2, FeatureType.Tree);
                    Enclose(level, TerrainType.StoneWall);
                    break;
                case 2: // Gorge
                    CellularAutomata ca = new CellularAutomata(level);
                    ca.WallType = TerrainType.StoneWall;
                    ca.FloorType = TerrainType.Grass;
                    ca.Run();
                    break;
                case 3:
                    goto case 0;
            }

            AmbientSpawner spawner = new AmbientSpawner(level, ValleyEnemies,
                ValleyEnemies, NPCPops._startingValley);
            spawner.Run();
        }

        public static void GenerateCentralValley(Level level)
        {
            ValleyBasics(level);

            // Never generate boss level if this is spawn
            if (level.LayerPos == Vector2Int.zero)
            {
                LevelRect rect = new LevelRect(new Vector2Int(0, 0),
                            new Vector2Int(
                                level.LevelSize.x,
                                level.LevelSize.y));

                FillRect(level, rect, FeatureType.WoodFence);
                BinarySpacePartition.BSP(level, TerrainType.Grass, 12);
                Enclose(level, TerrainType.StoneWall);
                foreach (Cell c in level.Map)
                    if (Utils.RandomUtils.OneChanceIn(6, true))
                        c.SetFeature(FeatureType.None); // Ruin fence

                if (level.LayerPos == Vector2Int.zero)
                {
                    UnityEngine.Debug.Log("Spawning the player...");
                    Game.instance.LoadLevel(level);
                    level.SpawnPlayer();
                }

                AmbientSpawner spawner = new AmbientSpawner(level,
                    ValleyEnemies, ValleyEnemies, NPCPops._startingValley);
                spawner.Run();

                return;
            }

            Enclose(level, TerrainType.StoneWall);
            Landmark.Build(LandmarkRef.Keep, level, 
                new Vector2Int(16, 16));
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
    }
}
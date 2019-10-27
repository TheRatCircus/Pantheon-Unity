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
            level.Map = BlankMap(level.LevelSize, ID.Terrain._grass);
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

                        FillRectTerrain(level, rect, ID.Feature._woodFence);
                        BinarySpacePartition.BSP(level, ID.Terrain._grass, 12);
                        Enclose(level, ID.Terrain._stoneWall);
                        foreach (Cell c in level.Map)
                            if (Utils.RandomUtils.OneChanceIn(6, true))
                                c.SetFeature(null); // Ruin fence
                        break;
                    }
                case 1: // Sparse wood
                    RandomFillFeature(level, 2, ID.Feature._tree);
                    Enclose(level, ID.Terrain._stoneWall);
                    break;
                case 2: // Gorge
                    CellularAutomata ca = new CellularAutomata(level);
                    ca.WallType = ID.Terrain._stoneWall;
                    ca.FloorType = ID.Terrain._grass;
                    ca.Run();
                    break;
                case 3: // Circle algorithm test
                    {
                        Enclose(level, ID.Terrain._stoneWall);
                        Utils.Algorithms.DrawCircle(40, 40, 32, 
                            (int x, int y) =>
                        {
                            level.GetCell(x, y).SetTerrain
                            (ID.Terrain._stoneWall);
                        });
                        break;
                    }
            }

            AmbientSpawner spawner = new AmbientSpawner(level, ValleyEnemies,
                ValleyEnemies, NPCPops._startingValley);
            spawner.Run();

            Items.SpawnItems(level);
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

                FillRectFeat(level, rect, ID.Feature._woodFence);
                BinarySpacePartition.BSP(level, ID.Terrain._grass, 12);
                Enclose(level, ID.Terrain._stoneWall);
                foreach (Cell c in level.Map)
                    if (Utils.RandomUtils.OneChanceIn(6, true))
                        c.SetFeature(null); // Ruin fence

                UnityEngine.Debug.Log("Spawning the player...");
                Game.instance.LoadLevel(level);
                level.SpawnPlayer();
            }
            else
            {
                Enclose(level, ID.Terrain._stoneWall);
                Landmark.Build(ID.Landmark._keep, level,
                    new Vector2Int(16, 16));
            }

            Items.SpawnItems(level);

            AmbientSpawner spawner = new AmbientSpawner(level,
                    ValleyEnemies, ValleyEnemies, NPCPops._startingValley);
            spawner.Run();
        }

        public static void PlaceGuaranteedAltar(Cell cell)
        {
            foreach (Idol idol in Game.Pantheon.Idols.Values)
                if (!idol.HasAnAltar)
                {
                    cell.SetAltar(new Altar(idol, idol.Aspect.AltarFeature));
                    return;
                }
        }
    }
}
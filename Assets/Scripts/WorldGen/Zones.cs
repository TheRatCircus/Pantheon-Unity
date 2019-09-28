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
        public const int ValleySize = 64;
        public const int ValleyEnemies = 10;

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

            UnityEngine.Debug.Log($"Initializing cells...");
            level.Map = BlankMap(level.LevelSize, TerrainType.Grass);
        }

        public static void GenerateValley(Level level)
        {
            int r = Utils.RandomUtils.RangeInclusive(0, 3);

            switch (r)
            {
                case 0: 
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
// CellularAutomata.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.World;
using static Pantheon.WorldGen.Layout;

namespace Pantheon.WorldGen
{
    public sealed class CellularAutomata
    {
        // Credit to Adam Rakaska

        public Level Level { get; private set; }
        public LevelRect Rect { get; private set; }

        public TerrainType WallType { get; set; }
        public TerrainType FloorType { get; set; }
        public int PercentAreWalls { get; private set; }

        public CellularAutomata(Level level, LevelRect rect = null,
            int percentAreWalls = 45)
        {
            Level = level;

            if (rect == null) // Create rect to fill whole level
                Rect = new LevelRect(
                    new Vector2Int(),
                    new Vector2Int(
                        Level.LevelSize.x - 1,
                        Level.LevelSize.y - 1));
            else
                Rect = rect;

            PercentAreWalls = percentAreWalls;
        }

        public void Run()
        {
            for (int iterations = 0; iterations < 10; iterations++)
            {
                RandomFillMap();
                Enclose(Level, Rect, WallType);
                MakeCaverns();

                if (FillDisconnected())
                    return;
            }

            UnityEngine.Debug.Log
                ("Cellular automata failed after 10 tries.");
        }

        private void MakeCaverns()
        {
            for (int column = Rect.x1, row = Rect.y1;
                row <= Rect.y2 - 1; row++)
                for (column = Rect.x1; column <= Rect.x2; column++)
                {
                    if (PlaceWallLogic(column, row))
                        Level.Map[column, row].SetTerrain
                            (WallType);
                    else
                        Level.Map[column, row].SetTerrain
                            (FloorType);
                }
        }

        private bool PlaceWallLogic(int x, int y)
        {
            int numWalls = 0;

            numWalls = Level.GetAdjacentWalls(Rect, x, y, 1, 1, true);

            if (Level.Map[x, y].Blocked)
            {
                if (numWalls >= 4)
                    return true;
                if (numWalls < 2)
                    return false;
            }
            else
            {
                if (numWalls >= 5)
                    return true;
            }
            return false;
        }

        private bool FillDisconnected()
        {
            int threshold = (int)(Rect.Width * Rect.Height * .4f);
            HashSet<Cell> cavern = new HashSet<Cell>();
            int attempts = 0;
            do
            {
                if (attempts > 50)
                {
                    UnityEngine.Debug.Log("No cavern of sufficient size" +
                        " found, regenerating...");
                    return false;
                }

                cavern = FloodFill(Level, Rect,
                    Level.RandomFloorInRect(Rect));
                attempts++;

            } while (cavern.Count < threshold);
            UnityEngine.Debug.Log("Cavern of " + cavern.Count + " found.");
            foreach (Cell cell in Level.CellsInRect(Rect))
                if (!cell.Blocked && !cavern.Contains(cell))
                    cell.SetTerrain(WallType);
            return true;
        }

        private void RandomFillMap()
        {
            int rectMiddle = 0;
            for (int column = Rect.x1, row = Rect.y1; row <= Rect.y2;
                row++)
            {
                for (column = Rect.x1; column <= Rect.x2; column++)
                {
                    rectMiddle = Rect.Center().y;

                    if (row == rectMiddle)
                        Level.Map[column, row].SetTerrain(FloorType);
                    else
                    {
                        if (Utils.RandomUtils.RangeInclusive(0, 100)
                            <= PercentAreWalls)
                            Level.Map[column, row].SetTerrain(WallType);
                        else
                            Level.Map[column, row].SetTerrain(FloorType);
                    }
                }
            }
        }
    }
}

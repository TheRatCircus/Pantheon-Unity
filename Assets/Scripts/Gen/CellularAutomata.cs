// CellularAutomata.cs
// Courtesy of Adam Rakaska

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Gen
{
    public sealed class CellularAutomata : BuilderStep
    {
        [JsonProperty] private readonly TerrainDefinition floor;
        [JsonProperty] private readonly TerrainDefinition wall;
        [JsonProperty] readonly int percentWalls = 45;

        private LevelRect rect;

        public CellularAutomata(string wall, string floor, int percentWalls)
        {
            this.wall = ScriptableObject.CreateInstance<TerrainDefinition>();
            this.wall.name = wall;
            this.floor = ScriptableObject.CreateInstance<TerrainDefinition>();
            this.floor.name = floor;
            this.percentWalls = percentWalls;
        }

        [JsonConstructor]
        public CellularAutomata(
            TerrainDefinition wall, TerrainDefinition floor, int percentWalls)
        {
            this.wall = wall;
            this.floor = floor;
            this.percentWalls = percentWalls;
        }

        public override void Run(Level level)
        {
            rect = new LevelRect(new Vector2Int(), new Vector2Int(
                        level.CellSize.x - 1, level.CellSize.y - 1));

            for (int iterations = 0; iterations < 10; iterations++)
            {
                RandomFillMap(level);
                Utils.Enclose(level, wall);
                MakeCaverns(level);

                if (FillDisconnected(level))
                    return;
            }

            throw new System.Exception("Cellular automata failed after 10 tries.");
        }

        private void MakeCaverns(Level level)
        {
            for (int column = rect.x1, row = rect.y1;
                row <= rect.y2 - 1; row++)
                for (column = rect.x1; column <= rect.x2; column++)
                {
                    if (PlaceWallLogic(level, column, row))
                        level.SetTerrain(column, row, wall);
                    else
                        level.SetTerrain(column, row, floor);
                }
        }

        private bool PlaceWallLogic(Level level, int x, int y)
        {
            int numWalls = level.GetAdjacentWalls(rect, x, y, 1, 1, true);

            if (level.CellIsWalled(new Vector2Int(x, y)))
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

        private bool FillDisconnected(Level level)
        {
            int threshold = (int)(rect.Width * rect.Height * .4f);
            HashSet<Vector2Int> cavern = new HashSet<Vector2Int>();
            int attempts = 0;
            do
            {
                if (attempts > 50)
                {
                    UnityEngine.Debug.Log("No cavern of sufficient size" +
                        " found, regenerating...");
                    
                    return false;
                }

                cavern = Floodfill.FillRect(level, rect, level.RandomFloorInRect(rect));
                attempts++;
            } while (cavern.Count < threshold);

            foreach (Vector2Int cell in level.CellsInRect(rect))
                if (!level.CellIsBlocked(cell) && !cavern.Contains(cell))
                    level.SetTerrain(cell.x, cell.y, wall);

            return true;
        }

        private void RandomFillMap(Level level)
        {
            int rectMiddle = 0;
            for (int column = rect.x1, row = rect.y1; row <= rect.y2;
                row++)
            {
                for (column = rect.x1; column <= rect.x2; column++)
                {
                    rectMiddle = rect.Center().y;

                    if (row == rectMiddle)
                        level.SetTerrain(column, row, floor);
                    else
                    {
                        if (RandomUtils.RangeInclusive(0, 100) <= percentWalls)
                            //level.Map[column, row].Wall = wall;
                            level.SetTerrain(column, row, wall);
                        else
                            level.SetTerrain(column, row, floor);
                    }
                }
            }
        }
    }
}

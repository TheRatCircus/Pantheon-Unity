// Layout.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.WorldGen
{
    public static class Layout
    {
        /// <summary>
        /// Initializes a blank cell map and fills it with a terrain type.
        /// </summary>
        /// <param name="size">The size of the new cell map.</param>
        /// <param name="terrain">The terrain to fill the new map with.</param>
        /// <returns>The new cell map.</returns>
        public static Cell[,] BlankMap(Vector2Int size, TerrainID terrain)
        {
            Cell[,] map = new Cell[size.x, size.y];
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = new Cell(new Vector2Int(x, y));
                    map[x, y].SetTerrain(terrain);
                }

            return map;
        }

        /// <summary>
        /// Enclose a level in walls.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="terrain">Terrain type with which to enclose the level.</param>
        public static void Enclose(Level level, TerrainID terrain)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                {
                    if (x == 0)
                    {
                        level.Map[x, y].SetTerrain(terrain);
                        continue;
                    }
                    else if (x == level.LevelSize.x - 1)
                    {
                        level.Map[x, y].SetTerrain(terrain);
                        continue;
                    }
                    else
                    {
                        if (y == 0)
                        {
                            level.Map[x, y].SetTerrain(terrain);
                            continue;
                        }
                        else if (y == level.LevelSize.y - 1)
                        {
                            level.Map[x, y].SetTerrain(terrain);
                            continue;
                        }
                    }
                }
        }

        public static void Enclose(Level level, LevelRect rect,
            TerrainID wallType)
        {
            for (int x = rect.x1; x <= rect.x2; x++)
                for (int y = rect.y1; y <= rect.y2; y++)
                {
                    if (x == rect.x1)
                    {
                        level.Map[x, y].SetTerrain(wallType);
                        continue;
                    }
                    else if (x == rect.x2)
                    {
                        level.Map[x, y].SetTerrain(wallType);
                        continue;
                    }
                    else
                    {
                        if (y == rect.y1)
                        {
                            level.Map[x, y].SetTerrain(wallType);
                            continue;
                        }
                        else if (y == rect.y2)
                        {
                            level.Map[x, y].SetTerrain(wallType);
                            continue;
                        }
                    }
                }
        }

        /// <summary>
        /// Fill a level's cells with terrain at a random percentage.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="percent">Likelihood that a cell gets filled.</param>
        /// <param name="terrain">Terrain type to fill the level with.</param>
        public static void RandomFill(Level level, int percent,
            TerrainID terrain)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                {
                    if (Game.PRNG.Next(0, 100) < percent)
                        level.Map[x, y].SetTerrain(terrain);
                }
        }

        /// <summary>
        /// Fill a level's cells with terrain at a random percentage.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="percent">Likelihood that a cell gets filled.</param>
        /// <param name="feature">Feature type to fill the level with.</param>
        public static void RandomFill(Level level, int percent,
            FeatureID feature)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                {
                    if (Game.PRNG.Next(0, 100) < percent)
                        if (!level.Map[x, y].Blocked)
                            level.Map[x, y].SetFeature(feature);
                }
        }

        /// <summary>
        /// Generate a number of randomly-sized rooms and then connect them 
        /// with tunnels at right angles.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="maxRooms">The maximum number of rooms.</param>
        /// <param name="roomMinSize">The minimum size of any given room.
        /// </param>
        /// <param name="roomMaxSize">The maximum size of any given room.
        /// </param>
        /// <returns></returns>
        public static void ConnectedRooms(Level level, int maxRooms,
            int roomMinSize, int roomMaxSize)
        {
            LevelRect[] rooms = new LevelRect[maxRooms];
            int numRooms = 0;

            // Center of last room for staircase
            Vector2Int lastCenter = new Vector2Int();

            for (int r = 0; r < maxRooms; r++)
            {
                Vector2Int pos = new Vector2Int();
                Vector2Int dims = new Vector2Int
                {
                    x = Game.PRNG.Next(roomMinSize, roomMaxSize),
                    y = Game.PRNG.Next(roomMinSize, roomMaxSize)
                };
                pos.x = Game.PRNG.Next(0, level.LevelSize.x - dims.x - 1);
                pos.y = Game.PRNG.Next(0, level.LevelSize.y - dims.y - 1);

                LevelRect newRoom = new LevelRect(pos, dims);

                bool overlaps = false;
                foreach (LevelRect otherRoom in rooms)
                {
                    if (newRoom.Intersects(otherRoom))
                        overlaps = true;
                }

                if (!overlaps)
                {
                    GenerateRoom(level, newRoom, TerrainID.StoneFloor);
                    Vector2Int newCenter = newRoom.Center();

                    lastCenter = newCenter;

                    if (numRooms >= 1)
                    {
                        Vector2Int prevCenter = rooms[numRooms - 1].Center();

                        if (Utils.RandomUtils.CoinFlip(true))
                        {
                            CreateHorizontalTunnel(level, prevCenter.x, newCenter.x, prevCenter.y);
                            CreateVerticalTunnel(level, prevCenter.y, newCenter.y, newCenter.x);
                        }
                        else
                        {
                            CreateVerticalTunnel(level, prevCenter.y, newCenter.y, prevCenter.x);
                            CreateHorizontalTunnel(level, prevCenter.x, newCenter.x, newCenter.y);
                        }
                    }
                    rooms[numRooms] = newRoom;
                    numRooms++;
                }
            }
        }

        /// <summary>
        /// Generate a room of floors given a rectangle.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="rect">The rectangle used to make the room.</param>
        /// <param name="terrain">The rectangle to make the room's floor with.</param>
        public static void GenerateRoom(Level level, LevelRect rect,
            TerrainID terrain)
        {
            //Debug.Log($"Generating room {rect.x2 - rect.x1} tiles wide and {rect.y2 - rect.y1} tiles long");
            for (int x = rect.x1 + 1; x < rect.x2 - 1; x++)
                for (int y = rect.y1 + 1; y < rect.y2 - 1; y++)
                    level.Map[x, y].SetTerrain(terrain);
        }

        public static void GenerateRoom(Level level, LevelRect rect,
            TerrainID wall, TerrainID floor)
        {
            FillRect(level, rect, wall);
            GenerateRoom(level, rect, floor);
        }

        public static void FillRect(Level level, LevelRect rect,
            TerrainID terrain)
        {
            for (int x = rect.x1; x < rect.x2; x++)
                for (int y = rect.y1; y < rect.y2; y++)
                {
                    if (level.Contains(new Vector2Int(x, y)))
                        level.Map[x, y].SetTerrain(terrain);
                }
        }

        public static void FillRect(Level level, LevelRect rect,
            FeatureID feature)
        {
            for (int x = rect.x1; x < rect.x2; x++)
                for (int y = rect.y1; y < rect.y2; y++)
                {
                    if (level.Contains(new Vector2Int(x, y)))
                        level.Map[x, y].SetFeature(feature);
                }
        }

        /// <summary>
        /// Create a tunnel in a horizontal direction.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="x1">Horizontal start of tunnel.</param>
        /// <param name="x2">Horizontal end of tunnel.</param>
        /// <param name="y">Y-position of tunnel.</param>
        private static void CreateHorizontalTunnel(Level level, int x1, int x2,
            int y)
        {
            for (int x = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2); x++)
                level.Map[x, y].SetTerrain(TerrainID.StoneFloor);
        }

        /// <summary>
        /// Create a tunnel in a vertical direction.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="y1">Vertical start of tunnel.</param>
        /// <param name="y2">Vertical end of tunnel.</param>
        /// <param name="x">X-position of tunnel.</param>
        private static void CreateVerticalTunnel(Level level, int y1, int y2,
            int x)
        {
            for (int y = Mathf.Min(y1, y2); y < Mathf.Max(y1, y2); y++)
                level.Map[x, y].SetTerrain(TerrainID.StoneFloor);
        }

        
    }
}

// Layout.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using static Pantheon.WorldGen.Layout;

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
        public static Cell[,] BlankMap(Vector2Int size, TerrainType terrain)
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
        public static void Enclose(Level level, TerrainType terrain)
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

        public static void Enclose(Level level, LevelRect rect, TerrainType wallType)
        {
            for (int x = rect.x1; x <= rect.x2; x++)
                for (int y = rect.y1; y <= rect.y2; y++)
                {
                    if (x == rect.x1)
                    {
                        level.Map[x, y].SetTerrain(wallType);
                        Debug.Visualisation.MarkPos(level.Map[x, y].Position, 120);
                        continue;
                    }
                    else if (x == rect.x2)
                    {
                        level.Map[x, y].SetTerrain(wallType);
                        Debug.Visualisation.MarkPos(level.Map[x, y].Position, 120);
                        continue;
                    }
                    else
                    {
                        if (y == rect.y1)
                        {
                            level.Map[x, y].SetTerrain(wallType);
                            Debug.Visualisation.MarkPos(level.Map[x, y].Position, 120);
                            continue;
                        }
                        else if (y == rect.y2)
                        {
                            level.Map[x, y].SetTerrain(wallType);
                            Debug.Visualisation.MarkPos(level.Map[x, y].Position, 120);
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
        public static void RandomFill(Level level, int percent, TerrainType terrain)
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
        public static void RandomFill(Level level, int percent, FeatureType feature)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                {
                    if (Game.PRNG.Next(0, 100) < percent)
                        if (!level.Map[x, y].Blocked)
                            level.Map[x, y].SetFeature(Database.GetFeature(feature));
                }
        }

        /// <summary>
        /// Generate a number of randomly-sized rooms and then connect them with
        /// tunnels at right angles.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="maxRooms">The maximum number of rooms.</param>
        /// <param name="roomMinSize">The minimum size of any given room.</param>
        /// <param name="roomMaxSize">The maximum size of any given room.</param>
        /// <returns></returns>
        public static void ConnectedRooms(Level level, int maxRooms, int roomMinSize, int roomMaxSize)
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
                    GenerateRoom(level, newRoom, TerrainType.StoneFloor);
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
        /// Generate a room given a rectangle.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="rect">The rectangle used to make the room.</param>
        /// <param name="terrain">The rectangle to make the room's floor with.</param>
        public static void GenerateRoom(Level level, LevelRect rect, TerrainType terrain)
        {
            //Debug.Log($"Generating room {rect.x2 - rect.x1} tiles wide and {rect.y2 - rect.y1} tiles long");
            for (int x = rect.x1 + 1; x < rect.x2 - 1; x++)
                for (int y = rect.y1 + 1; y < rect.y2 - 1; y++)
                    level.Map[x, y].SetTerrain(terrain);
        }

        public static void GenerateRoom(Level level, LevelRect rect,
            TerrainType wall, TerrainType floor)
        {
            FillRect(level, rect, wall);
            GenerateRoom(level, rect, floor);
        }

        public static void FillRect(Level level, LevelRect rect, TerrainType terrain)
        {
            for (int x = rect.x1; x < rect.x2; x++)
                for (int y = rect.y1; y < rect.y2; y++)
                {
                    if (level.Contains(new Vector2Int(x, y)))
                        level.Map[x, y].SetTerrain(terrain);
                }
        }

        public static void FillRect(Level level, LevelRect rect, FeatureType feature)
        {
            for (int x = rect.x1; x < rect.x2; x++)
                for (int y = rect.y1; y < rect.y2; y++)
                {
                    if (level.Contains(new Vector2Int(x, y)))
                        level.Map[x, y].SetFeature(Database.GetFeature(feature));
                }
        }

        /// <summary>
        /// Create a tunnel in a horizontal direction.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="x1">Horizontal start of tunnel.</param>
        /// <param name="x2">Horizontal end of tunnel.</param>
        /// <param name="y">Y-position of tunnel.</param>
        private static void CreateHorizontalTunnel(Level level, int x1, int x2, int y)
        {
            for (int x = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2); x++)
                level.Map[x, y].SetTerrain(TerrainType.StoneFloor);
        }

        /// <summary>
        /// Create a tunnel in a vertical direction.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="y1">Vertical start of tunnel.</param>
        /// <param name="y2">Vertical end of tunnel.</param>
        /// <param name="x">X-position of tunnel.</param>
        private static void CreateVerticalTunnel(Level level, int y1, int y2, int x)
        {
            for (int y = Mathf.Min(y1, y2); y < Mathf.Max(y1, y2); y++)
                level.Map[x, y].SetTerrain(TerrainType.StoneFloor);
        }

        public static HashSet<Cell> FloodFill(Level level, Cell start)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;


                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);
                            open.Add(frontierCell);
                        }
                    open.RemoveAt(i);
                }
            }
            return filled;
        }

        public static HashSet<Cell> FloodFill(Level level, LevelRect rect,
            Cell start)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier) && rect.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);
                            open.Add(frontierCell);
                        }
                    open.RemoveAt(i);
                }
            }
            return filled;
        }
    }

    /// <summary>
    /// An abstract rectangle in world space.
    /// </summary>
    public sealed class LevelRect
    {
        public int x1, x2, y1, y2;

        public int Width => x2 - x1;
        public int Height => y2 - y1;

        public LevelRect(Vector2Int pos, Vector2Int dims)
        {
            x1 = pos.x;
            y1 = pos.y;
            x2 = pos.x + dims.x;
            y2 = pos.y + dims.y;
        }

        public static bool IsNeighbour(LevelRect a, LevelRect b)
        {
            /// # DESCRIPTION
            /// Determine whether rectangles a and b are neighbors
            /// by projecting them onto both axes and comparing their
            /// combined projections ("one-dimensional shadows") to
            /// their actual sizes.
            /// If a projection:
            ///     - is smaller than both rectangles' width/height,
            ///     then the rectangles overlap on the x/ y - axis.
            ///     - is equivalent to both rectangles' width/height,
            ///     then the rectangles are touching on the x / y - axis.
            ///     - is greater than both rectangles' width/height,
            ///     then the rectangles can not be neighbors.
            /// 
            /// Return true iff the overlap on one axis is greater than zero
            /// while the overlap on the other axis is equal to zero.
            /// (If both overlaps were greater than zero, the rectangles
            /// would be overlapping. If both overlaps were equal to zero,
            /// the rectangles would be touching on a corner only.)

            int xProjection = Math.Max(a.x2, b.x2) - Math.Min(a.x1, b.x1);
            int xOverlap = a.Width + b.Width - xProjection;

            int yProjection = Math.Max(a.y2, b.y2) - Math.Min(a.y1, b.y1);
            int yOverlap = a.Height + b.Height - yProjection;

            return xOverlap > 0 && yOverlap == 0 ||
                xOverlap == 0 && yOverlap > 0;
        }

        public Vector2Int Center()
        {
            int centerX = (x1 + x2) / 2;
            int centerY = (y1 + y2) / 2;
            return new Vector2Int(centerX, centerY);
        }

        public bool Intersects(LevelRect other)
        {
            return (x1 <= other.x2 && x2 >= other.x1
                && y1 <= other.y2 && y2 >= other.y1);
        }

        public bool Contains(Vector2Int position)
        {
            return
                position.x >= x1 && position.y >= y1 &&
                position.x <= x2 && position.y <= y2;
        }

        public bool Contains(int x, int y)
        {
            return
                x >= x1 && y >= y1 && x <= x2 && y <= y2;
        }
    }
}

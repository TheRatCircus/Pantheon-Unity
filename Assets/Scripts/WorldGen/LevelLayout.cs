// LevelLayout.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Core;
using Pantheon.World;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for procedurally generating level layouts.
    /// </summary>
    public static class LevelLayout
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
                    map[x, y].SetTerrain(Database.GetTerrain(terrain));
                }

            return map;
        }

        /// <summary>
        /// Enclose a level in walls.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="terrain">Terrain type with which to enclose the level.</param>
        public static void Enclose(ref Level level, TerrainType terrain)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                {
                    if (x == 0)
                    {
                        level.Map[x, y].SetTerrain(Database.GetTerrain(terrain));
                        continue;
                    }
                    else if (x == level.LevelSize.x - 1)
                    {
                        level.Map[x, y].SetTerrain(Database.GetTerrain(terrain));
                        continue;
                    }
                    else
                    {
                        if (y == 0)
                        {
                            level.Map[x, y].SetTerrain(Database.GetTerrain(terrain));
                            continue;
                        }
                        else if (y == level.LevelSize.y - 1)
                        {
                            level.Map[x, y].SetTerrain(Database.GetTerrain(terrain));
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
        public static void RandomFill(ref Level level, int percent, TerrainType terrain)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                {
                    if (Game.PRNG().Next(0, 100) < percent)
                        level.Map[x, y].SetTerrain(Database.GetTerrain(terrain));
                }
        }

        /// <summary>
        /// Fill a level's cells with terrain at a random percentage.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="percent">Likelihood that a cell gets filled.</param>
        /// <param name="feature">Feature type to fill the level with.</param>
        public static void RandomFill(ref Level level, int percent, FeatureType feature)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                {
                    if (Game.PRNG().Next(0, 100) < percent)
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
        public static void ConnectedRooms(ref Level level, int maxRooms, int roomMinSize, int roomMaxSize)
        {
            Rectangle[] rooms = new Rectangle[maxRooms];
            int numRooms = 0;

            // Center of last room for staircase
            Vector2Int lastCenter = new Vector2Int();

            for (int r = 0; r < maxRooms; r++)
            {
                Vector2Int pos = new Vector2Int();
                Vector2Int dims = new Vector2Int
                {
                    x = Random.Range(roomMinSize, roomMaxSize),
                    y = Random.Range(roomMinSize, roomMaxSize)
                };
                pos.x = Random.Range(0, level.LevelSize.x - dims.x - 1);
                pos.y = Random.Range(0, level.LevelSize.y - dims.y - 1);

                Rectangle newRoom = new Rectangle(pos, dims);

                bool overlaps = false;
                foreach (Rectangle otherRoom in rooms)
                {
                    if (newRoom.Intersects(otherRoom))
                        overlaps = true;
                }

                if (!overlaps)
                {
                    GenerateRoom(ref level, newRoom, TerrainType.StoneFloor);
                    Vector2Int newCenter = newRoom.Center();

                    lastCenter = newCenter;

                    if (numRooms >= 1)
                    {
                        Vector2Int prevCenter = rooms[numRooms - 1].Center();

                        if (Random.Range(0, 2) == 1)
                        {
                            CreateHorizontalTunnel(ref level, prevCenter.x, newCenter.x, prevCenter.y);
                            CreateVerticalTunnel(ref level, prevCenter.y, newCenter.y, newCenter.x);
                        }
                        else
                        {
                            CreateVerticalTunnel(ref level, prevCenter.y, newCenter.y, prevCenter.x);
                            CreateHorizontalTunnel(ref level, prevCenter.x, newCenter.x, newCenter.y);
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
        public static void GenerateRoom(ref Level level, Rectangle rect, TerrainType terrain)
        {
            //Debug.Log($"Generating room {rect.x2 - rect.x1} tiles wide and {rect.y2 - rect.y1} tiles long");
            for (int x = rect.x1 + 1; x < rect.x2 - 1; x++)
                for (int y = rect.y1 + 1; y < rect.y2 - 1; y++)
                    level.Map[x, y].SetTerrain(Database.GetTerrain(terrain));
        }

        public static void FillRect(Level level, Rectangle rect, TerrainType terrain)
        {
            for (int x = rect.x1; x < rect.x2; x++)
                for (int y = rect.y1; y < rect.y2; y++)
                {
                    if (level.Contains(new Vector2Int(x, y)))
                        level.Map[x, y].SetTerrain(Database.GetTerrain(terrain));
                }   
        }

        /// <summary>
        /// Create a tunnel in a horizontal direction.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="x1">Horizontal start of tunnel.</param>
        /// <param name="x2">Horizontal end of tunnel.</param>
        /// <param name="y">Y-position of tunnel.</param>
        private static void CreateHorizontalTunnel(ref Level level, int x1, int x2, int y)
        {
            for (int x = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2); x++)
                level.Map[x, y].SetTerrain(Database.GetTerrain(TerrainType.StoneFloor));
        }

        /// <summary>
        /// Create a tunnel in a vertical direction.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="y1">Vertical start of tunnel.</param>
        /// <param name="y2">Vertical end of tunnel.</param>
        /// <param name="x">X-position of tunnel.</param>
        private static void CreateVerticalTunnel(ref Level level, int y1, int y2, int x)
        {
            for (int y = Mathf.Min(y1, y2); y < Mathf.Max(y1, y2); y++)
                level.Map[x, y].SetTerrain(Database.GetTerrain(TerrainType.StoneFloor));
        }

        /// <summary>
        /// An abstract rectangle in world space.
        /// </summary>
        public class Rectangle
        {
            public int x1, x2, y1, y2;

            /// <summary>
            /// Construct using Vector2Ints for position and dimensions.
            /// </summary>
            /// <param name="pos">The position of the upper-left corner.</param>
            /// <param name="dims">The size of the rectangle.</param>
            public Rectangle(Vector2Int pos, Vector2Int dims)
            {
                x1 = pos.x;
                y1 = pos.y;
                x2 = pos.x + dims.x;
                y2 = pos.y + dims.y;
            }

            /// <summary>
            /// Get the center of this rectangle.
            /// </summary>
            /// <returns>Rectangle's center.</returns>
            public Vector2Int Center()
            {
                int centerX = (x1 + x2) / 2;
                int centerY = (y1 + y2) / 2;
                return new Vector2Int(centerX, centerY);
            }

            /// <summary>
            /// Check if this rectangle intersects another.
            /// </summary>
            /// <param name="other">Other rectangle to check for intersection.</param>
            /// <returns>True if intersects.</returns>
            public bool Intersects(Rectangle other)
            {
                return (x1 <= other.x2 && x2 >= other.x1
                    && y1 <= other.y2 && y2 >= other.y1);
            }
        }
    }
}

// Functions for generating levels
using UnityEngine;

public class LevelGen
{
    // Generate a 2D Cell array representing a map
    public static Cell[,] GenerateLevel(ref Vector2Int spawnPoint,
        Vector2Int levelSize, int maxRooms, int roomMinSize, int roomMaxSize)
    {
        Cell[,] map = new Cell[levelSize.x, levelSize.y];
        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
                map[x, y] = new Cell(new Vector2Int(x, y));

        Rectangle[] rooms = new Rectangle[maxRooms];
        int numRooms = 0;

        for (int r = 0; r < maxRooms; r++)
        {
            Vector2Int pos = new Vector2Int();
            Vector2Int dims = new Vector2Int();
            dims.x = Random.Range(roomMinSize, roomMaxSize);
            dims.y = Random.Range(roomMinSize, roomMaxSize);
            pos.x = Random.Range(0, levelSize.x - dims.x - 1);
            pos.y = Random.Range(0, levelSize.y - dims.y - 1);

            Rectangle newRoom = new Rectangle(pos, dims);

            bool overlaps = false;
            foreach (Rectangle otherRoom in rooms)
            {
                if (newRoom.Intersects(otherRoom))
                    overlaps = true;
            }

            if (!overlaps)
            {
                GenerateRoom(ref map, newRoom);
                Vector2Int newCenter = newRoom.Center();

                if (numRooms == 0)
                {
                    spawnPoint = newCenter;
                }
                else
                {
                    Vector2Int prevCenter = rooms[numRooms - 1].Center();

                    if (Random.Range(0, 1) == 1)
                    {
                        CreateHorizontalTunnel(ref map, prevCenter.x, newCenter.x, prevCenter.y);
                        CreateVerticalTunnel(ref map, prevCenter.y, newCenter.y, newCenter.x);
                    }
                    else
                    {
                        CreateVerticalTunnel(ref map, prevCenter.y, newCenter.y, prevCenter.x);
                        CreateHorizontalTunnel(ref map, prevCenter.x, newCenter.x, newCenter.y);
                    }
                }
                rooms[numRooms] = newRoom;
                numRooms++;
            }
        }

        return map;
    }

    // Generate a room given a rectangle
    private static void GenerateRoom(ref Cell[,] map, Rectangle rect)
    {
        //Debug.Log($"Generating room {rect.x2 - rect.x1} tiles wide and {rect.y2 - rect.y1} tiles long");
        for (int x = rect.x1 + 1; x < rect.x2 - 1; x++)
            for (int y = rect.y1 + 1; y < rect.y2 - 1; y++)
            {
                map[x, y].Blocked = false;
                map[x, y].Opaque = false;
            }
    }

    // Create tunnel in a horizontal direction
    private static void CreateHorizontalTunnel(ref Cell[,] map, int x1, int x2, int y)
    {
        for (int x = Mathf.Min(x1, x2); x < Mathf.Max(x1, x2); x++)
        {
            map[x, y].Blocked = false;
            map[x, y].Opaque = false;
        }
    }

    // Create tunnel in a vertical direction
    private static void CreateVerticalTunnel(ref Cell[,] map, int y1, int y2, int x)
    {
        for (int y = Mathf.Min(y1, y2); y < Mathf.Max(y1, y2); y++)
        {
            map[x, y].Blocked = false;
            map[x, y].Opaque = false;
        }
    }

    // An abstract rectangle in world space, used to gen rooms
    public struct Rectangle
    {
        public int x1, x2, y1, y2;

        // Constructor
        public Rectangle(Vector2Int pos, Vector2Int dims)
        {
            x1 = pos.x;
            y1 = pos.y;
            x2 = pos.x + dims.x;
            y2 = pos.y + dims.y;
        }

        // Get the center of this rectangle
        public Vector2Int Center()
        {
            int centerX = (x1 + x2) / 2;
            int centerY = (y1 + y2) / 2;
            return new Vector2Int(centerX, centerY);
        }

        // Check if this rectangle intersects another
        public bool Intersects(Rectangle other)
        {
            return (x1 <= other.x2 && x2 >= other.x1
                && y1 <= other.y2 && y2 >= other.y1);
        }
    }
}

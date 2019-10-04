// Level.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.WorldGen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.World
{
    public sealed class Level : MonoBehaviour
    {
        // This level's tilemaps
        [SerializeField] private Tilemap terrainTilemap = null;
        [SerializeField] private Tilemap featureTilemap = null;
        [SerializeField] private Tilemap itemTilemap = null;
        [SerializeField] private Tilemap targettingTilemap = null;
        [SerializeField] private Tilemap splatterTilemap = null;

        public string DisplayName { get; set; } // Keep as null to highlight
        public string RefName { get; set; } // errors in world generation

        public Layer Layer { get; set; }
        public Vector2Int LayerPos { get; set; }

        public Cell[,] Map { get; set; }
        public Vector2Int LevelSize { get; set; }
        public List<NPC> NPCs { get; } = new List<NPC>();
        public Faction Faction { get; set; }

        // Connections
        public Dictionary<CardinalDirection, Connection> LateralConnections
        { get; set; } = new Dictionary<CardinalDirection, Connection>();
        public Connection[] UpConnections { get; set; }
        public Connection[] DownConnections { get; set; }

        private FOV fov;
        public void RefreshFOV() => fov.RefreshFOV(this);
        public Pathfinder Pathfinder { get; private set; }
        public DijkstraMap Autoexplore { get; private set; }

        public bool Visited { get; set; } = false;

        // Properties
        public Tilemap TerrainTilemap{ get => terrainTilemap; }
        public Tilemap FeatureTilemap { get => featureTilemap; }
        public Tilemap ItemTilemap { get => itemTilemap; }
        public Tilemap TargettingTilemap{ get => targettingTilemap; }
        public Tilemap SplatterTilemap { get => splatterTilemap; }
        
        private void Awake()
        {
            Pathfinder = new Pathfinder(this);
            Autoexplore = new DijkstraMap(this);
            fov = new FOV();
        }

        // Cell accessor, mostly for validation
        public Cell GetCell(Vector2Int pos)
        {
            if (Contains(pos))
                return Map[pos.x, pos.y];
            else
                throw new Exception
                    ($"Attempt to access out-of-bounds cell {pos.x}, {pos.y}");
        }

        public Cell GetCell(int x, int y)
        {
            if (Contains(new Vector2Int(x, y)))
                return Map[x, y];
            else
                throw new Exception
                    ($"Attempt to access out-of-bounds cell {x}, {y}");
        }

        // Put the player in their spawn position
        public void SpawnPlayer()
        {
            Actor.MoveTo(Game.GetPlayer(), RandomFloor());
            fov.RefreshFOV(this);
        }

        public Cell[,] CellsInRect(LevelRect rect)
        {
            Cell[,] rectMap = new Cell[rect.Width, rect.Height];
            for (int x = rect.x1, rectX = 0; x <= rect.x2 - 1; x++, rectX++)
                for (int y = rect.y1, rectY = 0; y <= rect.y2 - 1; y++,
                    rectY++)
                {
                    rectMap[rectX, rectY] = Map[x, y];
                }
            return rectMap;
        }

        // Find a random walkable cell in the level, no fixing
        public Cell RandomFloor()
        {
            Cell ret;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception("Could not find a random floor.");

                Vector2Int randomPosition = new Vector2Int
                {
                    x = UnityEngine.Random.Range(0, LevelSize.x),
                    y = UnityEngine.Random.Range(0, LevelSize.y)
                };

                ret = GetCell(randomPosition);
                attempts++;

            } while (ret.Blocked);
            return ret;
        }

        public Cell RandomFloorInRect(LevelRect rect)
        {
            Cell[,] rectMap = CellsInRect(rect);
            Cell ret;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception($"No random floor found in {rect}" +
                        " after 1000 tries.");

                int randX = UnityEngine.Random.Range(rect.x1, rect.x2);
                int randY = UnityEngine.Random.Range(rect.y1, rect.y2);

                ret = GetCell(randX, randY);

            } while (ret.Blocked);
            return ret;
        }

        public Cell RandomFloorOnX(int x, bool seeded)
        {
            Cell cell;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception
                        ($"Could not find a random floor at x {x}.");

                Vector2Int randomPosition = new Vector2Int
                {
                    x = x,
                    y = seeded ? Game.PRNG.Next(LevelSize.y) :
                    UnityEngine.Random.Range(0, LevelSize.y)
                };

                cell = GetCell(randomPosition);
                attempts++;

            } while (cell.Blocked);
            return cell;
        }

        public Cell RandomFloorOnY(int y, bool seeded)
        {
            Cell cell;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception
                        ($"Could not find a random floor at y {y}.");

                Vector2Int randomPosition = new Vector2Int
                {
                    x = seeded ? Game.PRNG.Next(LevelSize.x) :
                    UnityEngine.Random.Range(0, LevelSize.x),
                    y = y
                };

                cell = GetCell(randomPosition);
                attempts++;

            } while (cell.Blocked);
            return cell;
        }

        // Get a random floor beyond a certain distance from another point
        public Cell RandomFloorAwayFrom(Cell other, int distance)
        {
            Cell cell;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception
                        ($"Could not find a random floor at a distance of " +
                        $"{distance} to {other.Position}.");

                Vector2Int randomPosition = new Vector2Int
                {
                    x = UnityEngine.Random.Range(0, LevelSize.x),
                    y = UnityEngine.Random.Range(0, LevelSize.y)
                };

                cell = GetCell(randomPosition);
                attempts++;

            } while (cell.Blocked || Distance(cell, other) <= distance);
            return cell;
        }

        // Get the distance between two cells on this level
        public int Distance(Cell a, Cell b)
        {
            int dx = b.Position.x - a.Position.x;
            int dy = b.Position.y - a.Position.y;

            return (int)Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        }

        // Does this Level contain a point?
        public bool Contains(Vector2Int pos)
        {
            if (pos.x < LevelSize.x && pos.y < LevelSize.y)
                return (pos.x >= 0 && pos.y >= 0);
            else return false;
        }

        public bool Contains(int x, int y)
        {
            if (x < LevelSize.x && y < LevelSize.y)
                return (x >= 0 && y >= 0);
            else return false;
        }

        // Check if one cell is adjacent to another
        public bool AdjacentTo(Cell a, Cell b) => Distance(a, b) <= 1;

        // Get an adjacent cell given a direction
        public Cell GetAdjacentCell(Cell origin, Vector2Int delta)
        {
            if (delta.x == 0 && delta.y == 0)
                throw new ArgumentException
                    ("Level.GetAdjacentCell requires a non-zero delta");
            else if (delta.x > 1 || delta.y > 1)
                UnityEngine.Debug.LogWarning
                    ("GetAdjacentCell was passed a delta with a value" +
                    " greater than one");

            delta.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));

            Vector2Int newCellPos = origin.Position + delta;
            return GetCell(newCellPos);
        }

        /// <summary>
        /// Returns a number of blocked terrain cells adjacent to a cell.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scopeX"></param>
        /// <param name="scopeY"></param>
        /// <param name="oobIsWall">True to consider out-of-bounds as a wall.
        /// </param>
        /// <returns></returns>
        public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY,
            bool oobIsWall)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;
            int wallCounter = 0;

            int iX = startX;
            int iY = startY;

            for (iY = startY; iY <= endY; iY++)
                for (iX = startX; iX <= endX; iX++)
                    if (!(iX == x && iY == y))
                        if ((oobIsWall && !Contains(iX, iY))
                            || Map[iX, iY].Blocked)
                        {
                            wallCounter++;
                        }
            return wallCounter;
        }

        public int GetAdjacentWalls(LevelRect rect, int x, int y, int scopeX,
            int scopeY, bool oobIsWall)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;
            int wallCounter = 0;

            int iX = startX;
            int iY = startY;

            for (iY = startY; iY <= endY; iY++)
                for (iX = startX; iX <= endX; iX++)
                    if (!(iX == x && iY == y))
                        if ((oobIsWall && !Contains(iX, iY)
                            || (oobIsWall && !rect.Contains(iX, iY)))
                            || Map[iX, iY].Blocked)
                        {
                            wallCounter++;
                        }
            return wallCounter;
        }

        public Cell RandomAdjacentCell(Cell cell)
        {
            Vector2Int pos;

            do
            {
                int x = Utils.RandomUtils.RangeInclusive(-1, 1);
                int y = Utils.RandomUtils.RangeInclusive(-1, 1);
                pos = new Vector2Int(x, y);

            } while (!Contains(cell.Position + pos));

            return GetCell(cell.Position + pos);
        }

        public List<Cell> GetCellsAtX(int x, bool floorsOnly)
        {
            List<Cell> ret = new List<Cell>();
            for (int y = 0; y <= LevelSize.y - 1; y++)
            {
                Cell c = Map[x, y];
                if (!floorsOnly || !c.Blocked)
                    ret.Add(Map[x, y]);
            }
            return ret;
        }

        public List<Cell> GetCellsAtY(int y, bool floorsOnly)
        {
            List<Cell> ret = new List<Cell>();
            for (int x = 0; x <= LevelSize.x - 1; x++)
            {
                Cell c = Map[x, y];
                if (!floorsOnly || !c.Blocked)
                    ret.Add(Map[x, y]);
            }
            return ret;
        }

        public Cell RandomFloorInDirection(CardinalDirection direction)
        {
            Vector2Int vector = Utils.Helpers.CardinalToV2I(direction);
            switch (direction)
            {
                case CardinalDirection.North:
                    {
                        int y = LevelSize.y - 1; // Start at north
                        for (; y > 0; y--) // Iterate until south
                        {
                            List<Cell> cells = GetCellsAtY(y, true);
                            if (cells.Count > 0)
                            {
                                int r = UnityEngine.Random.Range
                                    (0, cells.Count);
                                return cells[r];
                            } // If all cells are blocked, move on
                            else continue;
                        }
                        break;
                    }
                case CardinalDirection.South:
                    {
                        int y = 0;
                        for (; y <= LevelSize.y; y++)
                        {
                            List<Cell> cells = GetCellsAtY(y, true);
                            if (cells.Count > 0)
                            {
                                int r = UnityEngine.Random.Range
                                    (0, cells.Count);
                                return cells[r];
                            }
                            else continue;
                        }
                    }
                    break;
                case CardinalDirection.East:
                    {
                        int x = LevelSize.x - 1;
                        for (; x > 0; x--)
                        {
                            List<Cell> cells = GetCellsAtX(x, true);
                            if (cells.Count > 0)
                            {
                                int r = UnityEngine.Random.Range
                                    (0, cells.Count);
                                return cells[r];
                            }
                            else continue;
                        }
                        break;
                    }
                case CardinalDirection.West:
                    {
                        int x = 0;
                        for (; x <= LevelSize.x; x++)
                        {
                            List<Cell> cells = GetCellsAtX(x, true);
                            if (cells.Count > 0)
                            {
                                int r = UnityEngine.Random.Range
                                    (0, cells.Count);
                                return cells[r];
                            }
                            else continue;
                        }
                        break;
                    }
                default:
                    throw new ArgumentException("Illegal direction.");
            }
            throw new Exception("Unknown error.");
        }

        public override string ToString()
            => RefName;
    }
}


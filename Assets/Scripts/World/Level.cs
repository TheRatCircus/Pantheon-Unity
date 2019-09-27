// Level.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.Core;
using Pantheon.Actors;

namespace Pantheon.World
{
    public class Level : MonoBehaviour
    {
        // This level's tilemaps
        [SerializeField] private Tilemap terrainTilemap;
        [SerializeField] private Tilemap featureTilemap;
        [SerializeField] private Tilemap itemTilemap;
        [SerializeField] private Tilemap targettingTilemap;

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

        public bool Visited { get; set; } = false;

        // Properties
        public Tilemap TerrainTilemap
        { get => terrainTilemap; private set => terrainTilemap = value; }
        public Tilemap FeatureTilemap
        { get => featureTilemap; private set => featureTilemap = value; }
        public Tilemap ItemTilemap
        { get => itemTilemap; private set => itemTilemap = value; }
        public Tilemap TargettingTilemap
        { get => targettingTilemap; private set => targettingTilemap = value; }
        
        private void Awake()
        {
            Pathfinder = new Pathfinder(this);
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

        // Find a random walkable cell in the level, no fixing
        public Cell RandomFloor()
        {
            Cell cell;
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

                cell = GetCell(randomPosition);
                attempts++;

            } while (!cell.IsWalkableTerrain());
            return cell;
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

            } while (!cell.IsWalkableTerrain());
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

            } while (!cell.IsWalkableTerrain());
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

            } while (!cell.IsWalkableTerrain() || Distance(cell, other) <= distance);
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
                    ("GetAdjacentCell was passed a delta with a value greater than one");

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
                            || Map[iX, iY].IsWall)
                        {
                            wallCounter++;
                        }
            return wallCounter;
        }

        public override string ToString()
            => RefName;
    }
}


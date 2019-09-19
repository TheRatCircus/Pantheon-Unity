// Level.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.WorldGen;

namespace Pantheon.World
{
    public struct LevelRef
    {
        public readonly string RefName;
        public Vector3Int Coords;
        public readonly Idol Idol;

        public LevelRef(string refName)
        {
            RefName = refName;
            Coords = new Vector3Int();
            Idol = null;
        }

        public LevelRef(string refName, Vector3Int coords, Idol idol)
        {
            RefName = refName;
            Coords = coords;
            Idol = idol;
        }

        /// <summary>
        /// Domain constructor.
        /// </summary>
        /// <param name="idol">The Idol of this Domain.</param>
        /// <param name="floor">The floor of the Idol's Domain.</param>
        public LevelRef(Idol idol, int floor)
        {
            RefName = $"domain{idol.DisplayName}{floor}";
            Idol = idol;
            Coords = new Vector3Int(0, 0, floor);
        }

        public LevelGenArgs ToGenArgs()
        {
            return new LevelGenArgs(Coords, Idol);
        }
    }

    public class Level : MonoBehaviour
    {
        // This level's tilemaps
        [SerializeField] private Tilemap terrainTilemap;
        [SerializeField] private Tilemap featureTilemap;
        [SerializeField] private Tilemap itemTilemap;
        [SerializeField] private Tilemap targettingTilemap;

        public string DisplayName { get; set; } = "NO_NAME";
        public LevelRef LevelRef { get; set; }
        public string RefName => LevelRef.RefName;

        public Layer Layer { get; set; }
        public Vector2Int LayerPos { get; set; }

        public Cell[,] Map { get; set; }
        public Vector2Int LevelSize { get; set; }
        public List<NPC> NPCs { get; } = new List<NPC>();

        // Connections
        public Dictionary<CardinalDirection, Connection> LateralConnections
        { get; set; } = new Dictionary<CardinalDirection, Connection>();
        public Connection[] UpConnections { get; set; }
        public Connection[] DownConnections { get; set; }

        private FOV fov;
        public void RefreshFOV() => fov.RefreshFOV(this);
        public Pathfinder Pathfinder { get; private set; }

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

        /// <summary>
        /// Find a random walkable cell; pass a positive int to one arg to fix
        /// the selection.
        /// </summary>
        /// <param name="x">Fix the selection by an x coordinate.</param>
        /// <param name="y">Fix the selection by a y coordinate.</param>
        /// <returns></returns>
        public Cell RandomFloor(int x, int y)
        {
            if (x > 0 && y > 0)
                UnityEngine.Debug.LogWarning("Result is fixed for both x and y.");

            Cell cell;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception
                        ($"Could not find a random floor at {x}, {y}.");

                Vector2Int randomPosition = new Vector2Int
                {
                    x = x < 0 ? UnityEngine.Random.Range(0, LevelSize.x) : x,
                    y = y < 0 ? UnityEngine.Random.Range(0, LevelSize.y) : y
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

        public override string ToString()
            => RefName;
    }
}


// Level.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Pantheon.World
{
    public sealed class Level : MonoBehaviour
    {
        private Tilemap terrain;
        private Tilemap splatter;
        private Tilemap items;

        public Dictionary<Vector2Int, Cell> Map { get; private set; }
            = new Dictionary<Vector2Int, Cell>();

        private Pathfinder pf;
        public Pathfinder PF => pf;

        public string DisplayName { get; private set; } = "DEFAULT_LEVEL_NAME";
        public string ID { get; private set; }

        public Vector3Int Position { get; private set; }
        public Vector2Int Size { get; set; }

        public Func<string, Object> AssetRequestEvent;
        public List<string> AssetIDCache { get; private set; }
            = new List<string>();

        void OnEnable()
        {
            Transform terrainTransform = transform.Find("Terrain");
            Transform splatterTransform = transform.Find("Splatter");
            Transform itemsTransform = transform.Find("Items");
            terrain = terrainTransform.GetComponent<Tilemap>();
            splatter = splatterTransform.GetComponent<Tilemap>();
            items = itemsTransform.GetComponent<Tilemap>();
        }

        public void RebuildPathfinder()
        {
            pf = new Pathfinder(this);
        }

        public bool TryGetCell(int x, int y, out Cell cell)
        {
            if (Map.TryGetValue(new Vector2Int(x, y), out cell))
                return true;
            else
                return false;
        }

        public bool TryGetCell(Vector2Int pos, out Cell cell)
        {
            if (Map.TryGetValue(pos, out cell))
                return true;
            else
                return false;
        }

        public Cell GetCell(Vector2Int pos)
        {
            if (Map.TryGetValue(pos, out Cell c))
                return c;
            else
                throw new ArgumentException(
                    $"Level {ID} has no cell at {pos}.");
        }

        public bool Contains(Vector2Int pos)
        {
            return Map.ContainsKey(pos);
        }

        public int Distance(Cell a, Cell b)
        {
            int dx = b.Position.x - a.Position.x;
            int dy = b.Position.y - a.Position.y;

            return (int)Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        }

        public bool AdjacentTo(Cell a, Cell b) => Distance(a, b) <= 1;

        public List<Cell> GetPathTo(Cell origin, Cell target)
            => PF.CellPathList(origin.Position, target.Position);

        public void Draw()
        {
            foreach (Cell c in Map.Values)
            {
                //UnityEngine.Debug.Log($"Drawing tile at {c.Position}...");
                terrain.SetTile((Vector3Int)c.Position, c.Terrain.Tile);
            }
        }

        public override string ToString() => $"{DisplayName} ({Position})";
    }
}

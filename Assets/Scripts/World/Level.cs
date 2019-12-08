// Level.cs
// Jerome Martina

using Pantheon.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Pantheon.World
{
    [Serializable]
    public sealed class Level
    {
        public string DisplayName { get; private set; } = "DEFAULT_LEVEL_NAME";
        public string ID { get; private set; }

        public Vector3Int Position { get; private set; }
        public Vector2Int Size { get; set; }

        public Dictionary<Vector2Int, Cell> Map { get; private set; }
            = new Dictionary<Vector2Int, Cell>();
        public Pathfinder PF { get; private set; }

        [NonSerialized] private Transform transform;
        public Transform Transform => transform;
        [NonSerialized] private Transform entitiesTransform;
        public Transform EntitiesTransform => entitiesTransform;

        [NonSerialized] private Tilemap terrain;
        [NonSerialized] private Tilemap splatter;
        [NonSerialized] private Tilemap items;

        public void AssignGameObject(Transform transform)
        {
            this.transform = transform;
            entitiesTransform = transform.Find("Entities");
            Transform terrainTransform = transform.Find("Terrain");
            Transform splatterTransform = transform.Find("Splatter");
            Transform itemsTransform = transform.Find("Items");
            terrain = terrainTransform.GetComponent<Tilemap>();
            splatter = splatterTransform.GetComponent<Tilemap>();
            items = itemsTransform.GetComponent<Tilemap>();
        }

        public void RebuildPathfinder()
        {
            PF = new Pathfinder(this);
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

        public bool AdjacentTo(Cell a, Cell b)
        {
            if (a.Equals(b))
                throw new ArgumentException("Argument cells are the same.");

            return Distance(a, b) <= 1;
        }

        public List<Cell> GetPathTo(Cell origin, Cell target)
            => PF.CellPathList(origin.Position, target.Position);

        public Cell RandomCell(bool open)
        {
            Cell cell;
            int tries = 0;
            do
            {
                if (tries >= 500)
                    throw new Exception(
                        $"No eligible cell found after {tries} attempts.");

                Vector2Int pos = new Vector2Int(Random.Range(0, Size.x),
                    Random.Range(0, Size.y));
                if (!TryGetCell(pos, out cell))
                    continue;

                if (!open || !cell.Blocked)
                    break;

                tries++;

            } while (true);
            return cell;
        }

        public void Draw(IEnumerable<Cell> cells)
        {
            foreach (Cell c in cells)
                DrawTile(c);
        }

        public void DrawTile(Cell cell)
        {
            if (cell.Revealed)
            {
                terrain.SetTile((Vector3Int)cell.Position, cell.Terrain.Tile);
                terrain.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);

                if (cell.Actor != null)
                    cell.Actor.GameObjects[0].SetSpriteVisibility(cell.Visible);
            }
        }

        public override string ToString() => $"{DisplayName} ({Position})";
    }
}

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
        public string DisplayName { get; set; } = "DEFAULT_LEVEL_NAME";
        public string ID { get; set; }

        public Vector3Int Position { get; private set; }
        public Vector2Int Size { get; set; }

        public Cell[,] Map { get; set; }
        public Pathfinder PF { get; private set; }

        [NonSerialized] private Transform transform;
        public Transform Transform => transform;
        [NonSerialized] private Transform entitiesTransform;
        public Transform EntitiesTransform => entitiesTransform;

        [NonSerialized] private Tilemap terrainTilemap;
        [NonSerialized] private Tilemap splatterTilemap;
        [NonSerialized] private Tilemap itemTilemap;

        public void AssignGameObject(Transform transform)
        {
            this.transform = transform;
            entitiesTransform = transform.Find("Entities");
            Transform terrainTransform = transform.Find("Terrain");
            Transform splatterTransform = transform.Find("Splatter");
            Transform itemsTransform = transform.Find("Items");
            terrainTilemap = terrainTransform.GetComponent<Tilemap>();
            splatterTilemap = splatterTransform.GetComponent<Tilemap>();
            itemTilemap = itemsTransform.GetComponent<Tilemap>();
        }

        public void RebuildPathfinder()
        {
            PF = new Pathfinder(this);
        }

        public bool TryGetCell(int x, int y, out Cell cell)
        {
            if (Contains(x, y))
            {
                cell = Map[x, y];
                return true;
            }
            else
            {
                cell = null;
                return false;
            }
        }

        public bool TryGetCell(Vector2Int pos, out Cell cell)
        {
            if (Contains(pos))
            {
                cell = Map[pos.x, pos.y];
                return true;
            }
            else
            {
                cell = null;
                return false;
            }
        }

        public Cell GetCell(Vector2Int pos)
        {
            if (Map[pos.x, pos.y] != null)
                return Map[pos.x, pos.y];
            else
                throw new ArgumentException(
                    $"Level {ID} has no cell at {pos}.");
        }

        public bool Contains(int x, int y)
        {
            if (x >= Size.x || y >= Size.y)
                return false;
            else if (x < 0 || y < 0)
                return false;
            else
                return Map[x, y] != null;
        }

        public bool Contains(Vector2Int pos)
        {
            if (pos.x >= Size.x || pos.y >= Size.y)
                return false;
            else if (pos.x < 0 || pos.y < 0)
                return false;
            else
                return Map[pos.x, pos.y] != null;
        }

        public int Distance(Cell a, Cell b)
        {
            int dx = b.Position.x - a.Position.x;
            int dy = b.Position.y - a.Position.y;

            return (int)Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        }

        public List<Cell> GetSquare(Cell origin, int radius)
        {
            int dim = (radius * 2) - 1;
            int delta = radius - 1;
            List<Cell> ret = new List<Cell>();
            for (int x = origin.Position.x - delta; x < origin.Position.x + delta; x++)
            {
                for (int y = origin.Position.y - delta; y < origin.Position.y + delta; y++)
                {
                    if (TryGetCell(x, y, out Cell c))
                        ret.Add(c);
                }
            }
            return ret;
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
                if (++tries >= 500)
                    throw new Exception(
                        $"No eligible cell found after {tries} attempts.");

                Vector2Int pos = new Vector2Int(Random.Range(0, Size.x),
                    Random.Range(0, Size.y));
                if (!TryGetCell(pos, out cell))
                    continue;

                if (!open || !cell.Blocked)
                    break;

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
                terrainTilemap.SetTile((Vector3Int)cell.Position,
                    cell.Terrain.Tile);
                terrainTilemap.SetColor((Vector3Int)cell.Position,
                    cell.Visible ? Color.white : Color.grey);

                if (cell.Actor != null)
                    cell.Actor.GameObjects[0].SetSpriteVisibility(cell.Visible);

                if (cell.Items.Count > 0)
                {
                    itemTilemap.SetTile((Vector3Int)cell.Position,
                        cell.Items[0].Flyweight.Tile);
                    itemTilemap.SetColor((Vector3Int)cell.Position,
                        cell.Visible ? Color.white : Color.grey);
                }
                else
                    itemTilemap.SetTile((Vector3Int)cell.Position, null);
            }
        }

        public override string ToString() => $"{DisplayName} ({Position})";
    }
}

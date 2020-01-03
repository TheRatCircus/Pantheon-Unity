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
            transform.gameObject.name = ID;
            this.transform = transform;
            entitiesTransform = transform.Find("Entities");
            Transform terrainTransform = transform.Find("Terrain");
            Transform splatterTransform = transform.Find("Splatter");
            Transform itemsTransform = transform.Find("Items");
            terrainTilemap = terrainTransform.GetComponent<Tilemap>();
            splatterTilemap = splatterTransform.GetComponent<Tilemap>();
            itemTilemap = itemsTransform.GetComponent<Tilemap>();
        }

        public void Initialize()
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

        /// <summary>
        /// Find a cell by position relative to an origin cell.
        /// </summary>
        /// <param name="origin">Cell to "translate" to another position.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Cell Translate(Cell origin, int x, int y)
        {
            int newX = origin.Position.x + x;
            int newY = origin.Position.y + y;

            if (newX < 0 || newX >= Map.GetLength(0))
                return null;
            if (newY < 0 || newY >= Map.GetLength(1))
                return null;

            return Map[newX, newY];
        }

        /// <summary>
        /// Find the nearest cell passing a predicate using a spiral algorithm.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="condition"></param>
        /// <returns>The nearest cell meeting the predicate, or null if none found.</returns>
        public Cell FindNearest(Cell origin, int radius, Predicate<Cell> condition)
        {
            Cell c = origin;
            if (condition(c))
                return c;

            for (int i = 0; i < radius; i++)
            {
                int j = i + 1;
                int k = j + 1;

                c = Translate(c, 0, 1); // Up
                if (condition(c))
                    return c;

                for (int right = 0; right < (i + j); right++)
                {
                    c = Translate(c, 1, 0);
                    if (condition(c))
                        return c;
                }

                for (int down = 0; down < (i + k); down++)
                {
                    c = Translate(c, 0, -1);
                    if (condition(c))
                        return c;
                }

                for (int left = 0; left < (i + k); left++)
                {
                    c = Translate(c, -1, 0);
                    if (condition(c))
                        return c;
                }

                for (int up = 0; up < (i + k); up++)
                {
                    c = Translate(c, 0, 1);
                    if (condition(c))
                        return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Enumerate all entities in an area by distance to an origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<Entity> FindBySpiral(Cell origin, int radius, Predicate<Entity> condition)
        {
            // Be aware that no null checks are performed on cell entities
            // so that the predicate can take nulls into account
            List<Entity> ret = new List<Entity>();
            Cell c = origin;
            if (condition(c.Actor))
                ret.Add(c.Actor);

            for (int i = 0; i < radius; i++)
            {
                int j = i + 1;
                int k = j + 1;

                c = Translate(c, 0, 1); // Up
                if (condition(c.Actor))
                    ret.Add(c.Actor);

                for (int right = 0; right < (i + j); right++)
                {
                    c = Translate(c, 1, 0);
                    if (condition(c.Actor))
                        ret.Add(c.Actor);
                }

                for (int down = 0; down < (i + k); down++)
                {
                    c = Translate(c, 0, -1);
                    if (condition(c.Actor))
                        ret.Add(c.Actor);
                }

                for (int left = 0; left < (i + k); left++)
                {
                    c = Translate(c, -1, 0);
                    if (condition(c.Actor))
                        ret.Add(c.Actor);
                }

                for (int up = 0; up < (i + k); up++)
                {
                    c = Translate(c, 0, 1);
                    if (condition(c.Actor))
                        ret.Add(c.Actor);
                }
            }
            return ret;
        }

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

                int randX = Random.Range(rect.x1, rect.x2);
                int randY = Random.Range(rect.y1, rect.y2);

                ret = Map[randX, randY];

            } while (ret.Wall != null);
            return ret;
        }

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
                            || Map[iX, iY].Wall != null)
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
                            || Map[iX, iY].Wall != null)
                        {
                            wallCounter++;
                        }
            return wallCounter;
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

        public void Draw(IEnumerable<Cell> cells)
        {
            foreach (Cell c in cells)
                DrawTile(c);
        }

        public void DrawTile(Cell cell)
        {
            if (cell.Revealed)
            {
                RuleTile terrainTile;
                if (cell.Wall != null)
                    terrainTile = cell.Wall.Tile;
                else if (cell.Ground != null)
                    terrainTile = cell.Ground.Tile;
                else
                    terrainTile = null;

                terrainTilemap.SetTile((Vector3Int)cell.Position, terrainTile);
                terrainTilemap.SetColor((Vector3Int)cell.Position,
                    cell.Visible ? Color.white : Color.grey);

                if (cell.Actor != null)
                    cell.Actor.GameObjects[0].SetSpriteVisibility(cell.Visible);

                if (cell.TryGetItem(0, out Entity item))
                {
                    itemTilemap.SetTile((Vector3Int)cell.Position,
                        item.Tile);
                    itemTilemap.SetColor((Vector3Int)cell.Position,
                        cell.Visible ? Color.white : Color.grey);
                }
                else
                    itemTilemap.SetTile((Vector3Int)cell.Position, null);
            }
        }

        public void ClearTilemaps()
        {
            terrainTilemap.ClearAllTiles();
            itemTilemap.ClearAllTiles();
            splatterTilemap.ClearAllTiles();
        }

        public override string ToString() => $"{DisplayName} {Position}";
    }
}

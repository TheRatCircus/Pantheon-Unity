// Level.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Content;
using Pantheon.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Pantheon.World
{
    [Serializable]
    public sealed class Level
    {
        public string DisplayName { get; set; } = "DEFAULT_LEVEL_NAME";
        public string ID { get; set; } = "DEFAULT_LEVEL_ID";

        public Vector3Int Position { get; set; }
        public Vector2Int ChunkSize { get; set; }
        public Vector2Int CellSize => new Vector2Int(
            Chunk.Width * ChunkSize.x,
            Chunk.Height * ChunkSize.y);
            
        public int CellCount => CellSize.x * CellSize.y;

        public Chunk[] Chunks { get; private set; }
        public Vector2Int[] Map
        {
            get
            {
                int i = 0;
                Vector2Int[] ret = new Vector2Int[CellCount];
                foreach (Chunk chunk in Chunks)
                {
                    for (int x = chunk.offsetX; x < chunk.offsetX + Chunk.Width - 1; x++)
                    {
                        for (int y = chunk.offsetY; y < chunk.offsetY + Chunk.Height - 1; y++)
                        {
                            ret[i++] = new Vector2Int(x, y);
                        }
                    }
                }
                return ret;
            }
        }
        public HashSet<Entity> Actors
        {
            get
            {
                HashSet<Entity> ret = new HashSet<Entity>();
                foreach (Chunk chunk in Chunks)
                {
                    if (chunk.Actors.Count < 1)
                        continue;

                    ret.AddMany(chunk.Actors);
                }
                return ret;
            }
        }

        public Pathfinder PF { get; private set; }

        [NonSerialized] private Transform transform;
        public Transform Transform => transform;
        [NonSerialized] private Transform entitiesTransform;
        public Transform EntitiesTransform => entitiesTransform;

        [NonSerialized] private Tilemap terrainTilemap;
        [NonSerialized] private Tilemap splatterTilemap;
        [NonSerialized] private Tilemap itemTilemap;

        public Level(Vector2Int size)
        {
            if (size.x % Chunk.Width != 0 || size.y % Chunk.Height != 0)
                throw new ArgumentException(
                    "Level size must conform to chunk grain.");

            ChunkSize = new Vector2Int(size.x / Chunk.Width, size.y / Chunk.Height);
            
            Chunks = new Chunk[ChunkSize.x * ChunkSize.y];
            for (int x = 0; x < ChunkSize.x; x++)
                for (int y = 0; y < ChunkSize.y; y++)
                    Chunks[Index(x, y)] = new Chunk(new Vector2Int(x, y));
        }

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

        public Chunk ChunkContaining(int x, int y)
        {
            int cX = x / Chunk.Width;
            int cY = y / Chunk.Height;

            Chunk ret = Chunks[(ChunkSize.x * cX) + cY];
            return ret;
        }

        public bool TryGetCell(int x, int y, out Cell cell)
        {
            if (Contains(x, y))
            {
                Chunk chunk = ChunkContaining(x, y);
                cell = new Cell(new CellHandle(chunk, (byte)x, (byte)y));
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
                Chunk chunk = ChunkContaining(pos.x, pos.y);
                cell = new Cell(new CellHandle(chunk, (byte)pos.x, (byte)pos.y));
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
            Chunk chunk = ChunkContaining(pos.x, pos.y);
            return new Cell(new CellHandle(chunk, (byte)pos.x, (byte)pos.y));
        }

        public Cell GetCell(int x, int y)
        {
            Chunk chunk = ChunkContaining(x, y);
            return new Cell(new CellHandle(chunk, (byte)x, (byte)y));
        }

        public bool Contains(int x, int y)
        {
            if (x >= CellSize.x || y >= CellSize.y)
                return false;
            else if (x < 0 || y < 0)
                return false;
            else
                return true;
        }

        public bool Contains(Vector2Int pos)
        {
            if (pos.x >= CellSize.x || pos.y >= CellSize.y)
                return false;
            else if (pos.x < 0 || pos.y < 0)
                return false;
            else
                return true;
        }

        public int Distance(Cell a, Cell b)
        {
            int dx = b.Position.x - a.Position.x;
            int dy = b.Position.y - a.Position.y;

            return (int)Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        }

        public List<Vector2Int> GetSquare(Vector2Int origin, int radius)
        {
            int dim = (radius * 2) - 1;
            int delta = radius - 1;
            List<Vector2Int> ret = new List<Vector2Int>();
            for (int x = origin.x - delta; x < origin.x + delta; x++)
            {
                for (int y = origin.y - delta; y < origin.y + delta; y++)
                {
                    if (Contains(x, y))
                        ret.Add(new Vector2Int(x, y));
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

        public List<Vector2Int> GetPathTo(Cell origin, Cell target)
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

            if (newX < 0 || newX >= CellSize.x)
                return null;
            if (newY < 0 || newY >= CellSize.y)
                return null;

            Chunk chunk = ChunkContaining(x, y);
            return new Cell(new CellHandle(chunk, (byte)newX, (byte)newY));
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

                Vector2Int pos = new Vector2Int(Random.Range(0, CellSize.x),
                    Random.Range(0, CellSize.y));
                if (!TryGetCell(pos, out cell))
                    continue;

                if (!open || !cell.Blocked)
                    break;

            } while (true);
            return cell;
        }

        public Cell RandomFloorInRect(LevelRect rect)
        {
            Cell ret;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception($"No random floor found in {rect}" +
                        " after 1000 tries.");

                int randX = Random.Range(rect.x1, rect.x2);
                int randY = Random.Range(rect.y1, rect.y2);

                Chunk chunk = ChunkContaining(randX, randY);
                ret = new Cell(new CellHandle(chunk, (byte)randX, (byte)randY));

            } while (ret.HasWall);
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
                            || GetTerrain(iX, iY).Blocked)
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
                            || GetTerrain(iX, iY).Blocked)
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
                    rectMap[rectX, rectY] = GetCell(x, y);
                }
            return rectMap;
        }

        public void Draw(IEnumerable<Vector2Int> cells)
        {
            foreach (Vector2Int cell in cells)
                DrawTile(cell);
        }

        public void DrawTile(Vector2Int cell)
        {
            Chunk chunk = ChunkContaining(cell.x, cell.y);

            if (!chunk.GetFlag(cell.x, cell.y).HasFlag(CellFlag.Revealed))
                return;

            bool visible = chunk.GetFlag(cell.x, cell.y).HasFlag(CellFlag.Visible);

            RuleTile terrainTile = chunk.GetTerrain(cell).Tile;

            terrainTilemap.SetTile((Vector3Int)cell, terrainTile);
            terrainTilemap.SetColor((Vector3Int)cell,
                visible ? Color.white : Color.grey);

            Entity actor = ActorAt(cell.x, cell.y);
            if (actor != null)
                actor.GameObjects[0].SetSpriteVisibility(visible);

            Entity item = FirstItemAt(cell.x, cell.y);
            if (item != null)
            {
                itemTilemap.SetTile((Vector3Int)cell, item.Tile);
                itemTilemap.SetColor((Vector3Int)cell,
                    visible ? Color.white : Color.grey);
            }
            else
                itemTilemap.SetTile((Vector3Int)cell, null);
        }

        public void ClearTilemaps()
        {
            terrainTilemap.ClearAllTiles();
            itemTilemap.ClearAllTiles();
            splatterTilemap.ClearAllTiles();
        }

        public override string ToString() => $"{DisplayName} {Position}";

        public int Index(int x, int y) => (ChunkSize.x * x) + y;

        public int Index(Vector2Int pos) => (ChunkSize.x * pos.x) + pos.y;

        public TerrainDefinition GetTerrain(int x, int y)
        {
            return ChunkContaining(x, y).GetTerrain(x, y);
        }

        public TerrainDefinition GetTerrain(Vector2Int pos)
        {
            return ChunkContaining(pos.x, pos.y).GetTerrain(pos);
        }

        public void SetTerrain(int x, int y, TerrainDefinition terrain)
        {
            ChunkContaining(x, y).SetTerrain(x, y, terrain);
        }

        public CellFlag GetFlag(int x, int y)
        {
            return ChunkContaining(x, y).GetFlag(x, y);
        }

        public void AddFlag(int x, int y, CellFlag flag)
        {
            Chunk chunk = ChunkContaining(x, y);
            chunk.Flags[chunk.Index(x, y)] |= flag;
        }

        public void RemoveFlag(int x, int y, CellFlag flag)
        {
            Chunk chunk = ChunkContaining(x, y);
            chunk.Flags[chunk.Index(x, y)] &= ~flag;
        }

        public List<Entity> ItemsAt(int x, int y)
        {
            return ChunkContaining(x, y).ItemsAt(x, y);
        }

        public Entity ActorAt(int x, int y)
        {
            return ChunkContaining(x, y).ActorAt(x, y);
        }

        public Entity ActorAt(Vector2Int pos)
        {
            return ChunkContaining(pos.x, pos.y).ActorAt(pos.x, pos.y);
        }

        public Entity FirstItemAt(int x, int y)
        {
            return ChunkContaining(x, y).FirstItemAt(x, y);
        }

        public Entity FirstItemAt(Vector2Int pos)
        {
            return ChunkContaining(pos.x, pos.y).FirstItemAt(pos);
        }

        public void MoveEntityTo(Entity entity, Vector2Int pos)
        {
            if (entity.HasComponent<Actor>())
                ChunkContaining(pos.x, pos.y).Actors.Add(entity);
            else
                ChunkContaining(pos.x, pos.y).Items.Add(entity);
        }

        public bool CellTerrainIsBlocked(Vector2Int pos)
        {
            TerrainDefinition def = GetTerrain(pos);
            return def != null && def.Blocked;
        }

        public bool CellIsOpaque(Vector2Int pos)
        {
            TerrainDefinition def = GetTerrain(pos);
            return def != null && def.Opaque;
        }

        public bool CellHasActor(Vector2Int pos)
        {
            return ChunkContaining(pos.x, pos.y).CellHasActor(pos);
        }
    }
}

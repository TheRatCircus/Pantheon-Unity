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
        // The offset of each tile from Unity's true grid coords
        public const float TileOffsetX = .5f;
        public const float TileOffsetY = .5f;

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

        public static Vector2Int NullCell => new Vector2Int(-1, -1);

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

        public static int Distance(Vector2Int a, Vector2Int b)
        {
            int dx = b.x - a.x;
            int dy = b.y - a.y;

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

        public bool AdjacentTo(Vector2Int a, Vector2Int b)
        {
            if (a.Equals(b))
                throw new ArgumentException("Argument cells are the same.");

            // TODO: == 1
            return Distance(a, b) <= 1;
        }

        public Line GetPathTo(Vector2Int origin, Vector2Int target)
            => PF.CellPathList(origin, target);

        /// <summary>
        /// Find a cell by position relative to an origin cell.
        /// </summary>
        /// <param name="origin">Cell to "translate" to another position.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector2Int Translate(Vector2Int origin, int x, int y)
        {
            int newX = origin.x + x;
            int newY = origin.y + y;

            if (newX < 0 || newX >= CellSize.x)
                return NullCell;
            if (newY < 0 || newY >= CellSize.y)
                return NullCell;

            return new Vector2Int(newX, newY);
        }

        /// <summary>
        /// Enumerate all entities in an area by distance to an origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<Entity> FindBySpiral(Vector2Int origin, int radius, Predicate<Entity> condition)
        {
            // Be aware that no null checks are performed on cell entities
            // so that the predicate can take nulls into account
            List<Entity> ret = new List<Entity>();
            Vector2Int c = origin;
            Entity e = ActorAt(c);
            if (condition(e))
                ret.Add(e);

            for (int i = 0; i < radius; i++)
            {
                int j = i + 1;
                int k = j + 1;

                c = Translate(c, 0, 1); // Up
                e = ActorAt(c);
                if (condition(e))
                    ret.Add(e);

                for (int right = 0; right < (i + j); right++)
                {
                    c = Translate(c, 1, 0);
                    e = ActorAt(c);
                    if (condition(e))
                        ret.Add(e);
                }

                for (int down = 0; down < (i + k); down++)
                {
                    c = Translate(c, 0, -1);
                    e = ActorAt(c);
                    if (condition(e))
                        ret.Add(e);
                }

                for (int left = 0; left < (i + k); left++)
                {
                    c = Translate(c, -1, 0);
                    e = ActorAt(c);
                    if (condition(e))
                        ret.Add(e);
                }

                for (int up = 0; up < (i + k); up++)
                {
                    c = Translate(c, 0, 1);
                    e = ActorAt(c);
                    if (condition(e))
                        ret.Add(e);
                }
            }
            return ret;
        }

        public Vector2Int RandomCell(bool open)
        {
            Vector2Int cell;
            int tries = 0;
            do
            {
                if (++tries >= 500)
                    throw new Exception(
                        $"No eligible cell found after {tries} attempts.");

                cell = new Vector2Int(Random.Range(0, CellSize.x),
                    Random.Range(0, CellSize.y));

                if (!open || !CellIsBlocked(cell))
                    break;

            } while (true);
            return cell;
        }

        public Vector2Int RandomFloorInRect(LevelRect rect)
        {
            Vector2Int ret;
            int attempts = 0;
            do
            {
                if (attempts > 1000)
                    throw new Exception($"No random floor found in {rect}" +
                        " after 1000 tries.");

                int randX = Random.Range(rect.x1, rect.x2);
                int randY = Random.Range(rect.y1, rect.y2);

                ret = new Vector2Int(randX, randY);

            } while (CellIsWalled(ret));
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

        public Vector2Int[,] CellsInRect(LevelRect rect)
        {
            Vector2Int[,] rectMap = new Vector2Int[rect.Width, rect.Height];
            for (int x = rect.x1, rectX = 0; x <= rect.x2 - 1; x++, rectX++)
                for (int y = rect.y1, rectY = 0; y <= rect.y2 - 1; y++,
                    rectY++)
                {
                    rectMap[rectX, rectY] = new Vector2Int(x, y);
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

        public void SetVisibility(int x, int y, bool visible)
        {
            if (visible)
            {
                AddFlag(x, y, CellFlag.Visible);
                AddFlag(x, y, CellFlag.Revealed);
            }
            else
                RemoveFlag(x, y, CellFlag.Visible);
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

        public bool CellIsBlocked(Vector2Int pos)
        {
            if (CellIsWalled(pos))
                return true;

            if (CellHasActor(pos))
                return true;

            return false;
        }

        public bool CellIsWalled(Vector2Int pos)
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

        public bool Walkable(Vector2Int pos)
        {
            return !CellIsWalled(pos);
        }

        public bool CellIsVisible(Vector2Int pos)
        {
            return ChunkContaining(pos.x, pos.y).CellIsVisible(pos);
        }
    }
}

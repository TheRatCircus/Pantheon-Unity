// Level.cs
// Jerome Martina

#define DEBUG_DRAW
#undef DEBUG_DRAW

using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        public const float TileOffset = 0.5f;

        public static readonly Vector2Int NullCell = new Vector2Int(-1, -1);

        public string DisplayName { get; set; } = "DEFAULT_LEVEL_NAME";
        public string ID { get; set; } = "DEFAULT_LEVEL_ID";

        public Vector3Int Position { get; private set; }
        public Vector2Int Size { get; } // TODO: Get Map lengths
        public int CellCount => Size.x * Size.y;

        private readonly byte[] terrainMap;
        private readonly CellFlag[] flagMap;
        public IEnumerable<Vector2Int> Map
        {
            get
            {
                for (int y = 0; y < Size.y; y++)
                    for (int x = 0; x < Size.x; x++)
                        yield return new Vector2Int(x, y);
            }
        }
        [NonSerialized] private Pathfinder pathfinder;
        public Pathfinder Pathfinder
        {
            get => pathfinder;
            private set => pathfinder = value;
        }

        private readonly Dictionary<Vector2Int, Entity> actors
            = new Dictionary<Vector2Int, Entity>();
        private readonly Dictionary<Vector2Int, List<Entity>> items
            = new Dictionary<Vector2Int, List<Entity>>();
        public Dictionary<Vector2Int, Connection> Connections { get; }
            = new Dictionary<Vector2Int, Connection>(1);

        [NonSerialized] private Transform transform;
        [NonSerialized] private Transform entitiesTransform;
        public Transform Transform => transform;
        public Transform EntitiesTransform => entitiesTransform;

        [NonSerialized] private Tilemap terrainTilemap;
        [NonSerialized] private Tilemap featureTilemap;
        [NonSerialized] private Tilemap splatterTilemap;
        [NonSerialized] private Tilemap itemTilemap;

        public Level(int sizeX, int sizeY)
        {
            terrainMap = new byte[sizeX * sizeY];
            flagMap = new CellFlag[sizeX * sizeY];
            Size = new Vector2Int(sizeX, sizeY);
        }

        public void AssignGameObject(Transform transform)
        {
            transform.gameObject.name = ID;
            this.transform = transform;
            entitiesTransform = transform.Find("Entities");
            terrainTilemap = transform.Find("Terrain").GetComponent<Tilemap>();
            featureTilemap = transform.Find("Features").GetComponent<Tilemap>();
            splatterTilemap = transform.Find("Splatter").GetComponent<Tilemap>();
            itemTilemap = transform.Find("Items").GetComponent<Tilemap>();
        }

        public void Initialize() => Pathfinder = new Pathfinder(this);

        private int MapCoords(int x, int y) => Size.x * x + y;

        public TerrainDefinition GetTerrain(int x, int y)
        {
            byte index = terrainMap[MapCoords(x, y)];
            return Assets.GetTerrain(index);
        }

        public void SetTerrain(int x, int y, TerrainDefinition terrain)
        {
            byte index = Assets.GetTerrainIndex(terrain);
            terrainMap[MapCoords(x, y)] = index;
        }

        public bool Visible(int x, int y)
        {
            return flagMap[MapCoords(x, y)].HasFlag(CellFlag.Visible);
        }

        public bool Revealed(int x, int y)
        {
            return flagMap[MapCoords(x, y)].HasFlag(CellFlag.Revealed);
        }

        public bool Blocked(int x, int y)
        {
            Vector2Int v = new Vector2Int(x, y);
            TerrainDefinition terrain = GetTerrain(x, y);

            // TODO: This is to prevent a floorless cell from being considered 
            // walkable, but in the future this should allow movement via flight
            return terrain == null || terrain.Blocked || actors.ContainsKey(v);
        }

        public bool Opaque(int x, int y) => GetTerrain(x, y).Opaque;

        public bool Contains(int x, int y)
        {
            return
                x < Size.x &&
                y < Size.y &&
                x >= 0 &&
                y >= 0;
        }

        public bool Contains(Vector2Int position)
        {
            return
                position.x < Size.x &&
                position.y < Size.y &&
                position.x >= 0 &&
                position.y >= 0;
        }

        public bool Walled(int x, int y) => GetTerrain(x, y).Blocked;

        public bool Walled(Vector2Int cell)
        {
            return GetTerrain(cell.x, cell.y).Blocked;
        }

        public bool Walkable(Vector2Int cell)
        {
            TerrainDefinition terrain = GetTerrain(cell.x, cell.y);
            return 
                Contains(cell) && 
                ActorAt(cell) == null && 
                terrain != null &&
                !terrain.Blocked;
        }

        public bool SetVisibility(int x, int y, bool visible)
        {
            int map = MapCoords(x, y);

            bool change = 
                (!Visible(x, y) && visible) || 
                (Visible(x, y) && !visible);

            if (visible)
                flagMap[map] |= CellFlag.Visible | CellFlag.Revealed;
            else
                flagMap[map] &= ~CellFlag.Visible;

            return change;
        }

        public void Reveal(int x, int y)
        {
            int map = MapCoords(x, y);
            flagMap[map] |= CellFlag.Revealed;
        }

        public Entity ActorAt(Vector2Int cell)
        {
            if (actors.TryGetValue(cell, out Entity ret))
                return ret;
            else
                return null;
        }

        public List<Entity> ItemsAt(int x, int y)
        {
            if (items.TryGetValue(new Vector2Int(x, y), out List<Entity> ret))
                return ret;
            else
                return new List<Entity>(0);
        }

        public void ClearEntity(Entity entity)
        {
            // Just attempt both
            actors.Remove(entity.Cell);
            RemoveItem(entity);
        }

        public void MoveEntity(Entity entity, Vector2Int from, Vector2Int to)
        {
            if (actors.Remove(from) || entity.HasComponent<Actor>())
                actors.Add(to, entity);
            else if (RemoveItem(entity))
                AddItem(entity, to);
            else // Fall back to assuming this is an item
                AddItem(entity, to);
        }

        private void AddItem(Entity item, Vector2Int position)
        {
            if (items.TryGetValue(position, out List<Entity> existing))
                existing.Add(item);
            else
                items.Add(position, new List<Entity>() { item });
        }

        private bool RemoveItem(Entity item)
        {
            if (items.TryGetValue(item.Cell, out List<Entity> list))
                return list.Remove(item);
            else
                return false;
        }

        public Line GetPath(Vector2Int start, Vector2Int end)
        {
            return Pathfinder.GetPath(start, end);
        }

        public List<Vector2Int> CellsInRect(LevelRect rect)
        {
            List<Vector2Int> ret = new List<Vector2Int>();
            for (int x = rect.x1, rectX = 0; x <= rect.x2 - 1; x++, rectX++)
                for (int y = rect.y1, rectY = 0; y <= rect.y2 - 1; y++,
                    rectY++)
                {
                    ret.Add(new Vector2Int(x, y));
                    //ret[rectX, rectY] = Map[x, y];
                }
            return ret;
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
                    Vector2Int v = new Vector2Int(x, y);
                    if (Contains(v)) ret.Add(v);
                }
            }
            return ret;
        }

        public int AdjacentWallCount(int x, int y, int scopeX, int scopeY,
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
                            || Walled(iX, iY))
                        {
                            wallCounter++;
                        }

            return wallCounter;
        }

        public int AdjacentWallCount(LevelRect rect, int x, int y, int scopeX,
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
                            || Walled(iX, iY))
                        {
                            wallCounter++;
                        }
            return wallCounter;
        }

        public void Draw(IEnumerable<Vector2Int> cells)
        {
            foreach (Vector2Int cell in cells)
                DrawTile(cell);
        }

        public void DrawTile(Vector2Int cell)
        {
            if (!Revealed(cell.x, cell.y))
            {
                Locator.Scheduler.UnmarkCell(cell);
                return;
            }

            Profiler.BeginSample("Level.DrawTile()");

            RuleTile terrainTile;
            TerrainDefinition terrain = GetTerrain(cell.x, cell.y);
            if (terrain != null)
                terrainTile = terrain.Tile;
            else
                terrainTile = null;

            bool visible = Visible(cell.x, cell.y);

            if (terrainTilemap.GetTile((Vector3Int)cell) != terrainTile)
                terrainTilemap.SetTile((Vector3Int)cell, terrainTile);
            terrainTilemap.SetColor((Vector3Int)cell,
                visible ? Color.white : Color.grey);

            if (actors.TryGetValue(cell, out Entity actor))
                actor.GameObjects[0].SetSpriteVisibility(visible);

            List<Entity> items = ItemsAt(cell.x, cell.y);
            if (items.Count > 0)
            {
                itemTilemap.SetTile((Vector3Int)cell, items[0].Tile);
                itemTilemap.SetColor((Vector3Int)cell,
                    visible ? Color.white : Color.grey);
            }
            else
                itemTilemap.SetTile((Vector3Int)cell, null);

            if (Connections.TryGetValue(cell, out Connection conn))
                featureTilemap.SetTile((Vector3Int)cell, conn.Tile);

            if (!visible)
                Locator.Scheduler.UnmarkCell(cell);

            Profiler.EndSample();
        }

        public Vector2Int RandomUnwalledCell()
        {
            Vector2Int ret;
            int attempts = 0;
            do
            {
                if (attempts > 500)
                    throw new Exception(
                        $"Failed {attempts} times to get a random cell.");

                ret = new Vector2Int(
                    Random.Range(0, Size.x),
                    Random.Range(0, Size.y));

                attempts++;
            } while (Walled(ret));
            return ret;
        }

        public Vector2Int RandomCell(Predicate<Vector2Int> predicate)
        {
            Vector2Int ret;
            int attempts = 0;
            do
            {
                if (attempts > 500)
                    throw new Exception(
                        $"Failed {attempts} times to get a random cell.");

                ret = new Vector2Int(
                    Random.Range(0, Size.x),
                    Random.Range(0, Size.y));

                attempts++;
            } while (!predicate(ret));
            return ret;
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

            } while (Walled(ret));
            return ret;
        }

        public Vector2Int RandomCorner()
        {
            int r = Random.Range(0, 4);
            switch (r)
            {
                case 0: // Northeast
                    return new Vector2Int(Size.x - 1, Size.y - 1);
                case 1: // Southeast
                    return new Vector2Int(Size.x - 1, 0);
                case 2: // Southwest
                    return new Vector2Int(0, 0);
                case 3: // Northwest
                    return new Vector2Int(0, Size.y - 1);
                default:
                    throw new Exception(
                        "Random.Range() returned illegal value.");
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctxt)
        {
            Pathfinder = null;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctxt)
        {
            Pathfinder = new Pathfinder(this);
        }

        public string CellToString(Vector2Int cell)
        {
            return $"{GetTerrain(cell.x, cell.y)} {cell}";
        }

        public override string ToString() => $"{DisplayName} {Position}";
    }
}

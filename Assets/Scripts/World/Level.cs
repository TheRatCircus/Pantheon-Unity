// Level.cs
// Jerome Martina

using Pantheon.ECS.Templates;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Pantheon.World
{
    [Serializable]
    public sealed class Level
    {
        [NonSerialized] private GameObject levelObj;
        public GameObject LevelObj => levelObj;

        [NonSerialized] private Tilemap terrain;
        [NonSerialized] private Tilemap splatter;
        [NonSerialized] private Tilemap items;

        public Dictionary<Vector2Int, Cell> Map { get; private set; }
            = new Dictionary<Vector2Int, Cell>();

        [NonSerialized] private Pathfinder pf;
        public Pathfinder PF => pf;

        public string DisplayName { get; private set; } = "DEFAULT_LEVEL_NAME";
        public string ID { get; private set; }

        public Vector3Int Position { get; private set; }
        public Vector2Int Size { get; set; }

        public Func<string, Object> AssetRequestEvent;
        public List<string> AssetIDCache { get; private set; }
            = new List<string>();

        public Level(string displayName, string id)
        {
            DisplayName = displayName;
            ID = id;
        }

        public void SetLevelObject(GameObject levelObj)
        {
            this.levelObj = levelObj;
            Transform terrainTransform = levelObj.transform.Find("Terrain");
            Transform splatterTransform = levelObj.transform.Find("Splatter");
            Transform itemsTransform = levelObj.transform.Find("Items");
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

        public List<Cell> GetPathTo(Cell origin, Cell target)
            => PF.CellPathList(origin.Position, target.Position);

        public void VisualizeTile(Cell cell)
        {
            if (cell.Terrain != null)
            {
                if (!cell.Revealed)
                    return;
                else
                {
                    terrain.SetTile((Vector3Int)cell.Position,
                        cell.Terrain.Flyweight.Tile);
                    terrain.SetColor((Vector3Int)cell.Position,
                        cell.Visible ? Color.white : Color.grey);
                }
            }
        }

        [OnSerializing]
        private void OnSerializing()
        {
            AssetRequestEvent = null;
        }

        public override string ToString() => $"{DisplayName} ({Position})";
    }
}

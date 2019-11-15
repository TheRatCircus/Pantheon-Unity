// Level.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Pantheon.ECS.Templates;
using System.Runtime.Serialization;

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

        public void VisualizeTile(Cell cell)
        {
            if (cell.Terrain != null)
            {
                // Re-assign flyweights if level is being loaded from a save
                if (cell.Terrain.Flyweight == null)
                    cell.Terrain.Flyweight = (Template)AssetRequestEvent.Invoke(cell.Terrain.FlyweightID);

                terrain.SetTile((Vector3Int)cell.Position, cell.Terrain.Flyweight.Tile);
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

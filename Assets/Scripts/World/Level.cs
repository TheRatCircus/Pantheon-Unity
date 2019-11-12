// Level.cs
// Jerome Martina

using Pantheon.ECS.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.World
{
    [System.Serializable]
    public sealed class Level
    {
        public GameObject LevelObject { get; private set; }

        public Tilemap Terrain { get; private set; } = default;
        public Tilemap Splatter { get; private set; } = default;
        public Tilemap Items { get; private set; } = default;

        public Dictionary<Vector2Int, Cell> Map { get; private set; }
            = new Dictionary<Vector2Int, Cell>();

        public string DisplayName { get; private set; } = "DEFAULT_LEVEL_NAME";
        public string ID { get; private set; }

        public Vector3Int Position { get; set; }
        public Vector2Int Size { get; set; }

        public event System.Action<Level> FirstEntryEvent;

        public Level(string displayName, string id)
        {
            DisplayName = displayName;
            ID = id;
        }

        public void SetLevelObject(GameObject levelObj)
        {
            LevelObject = levelObj;
            Transform terrainTransform = levelObj.transform.Find("Terrain");
            Transform splatterTransform = levelObj.transform.Find("Splatter");
            Transform itemsTransform = levelObj.transform.Find("Items");
            Terrain = terrainTransform.GetComponent<Tilemap>();
            Splatter = splatterTransform.GetComponent<Tilemap>();
            Items = itemsTransform.GetComponent<Tilemap>();
        }

        public bool TryGetCell(int x, int y, out Cell cell)
        {
            //if (Map[x, y] == null)
            //{
            //    cell = null;
            //    return false;
            //}
            //else
            //{
            //    cell = Map[x, y];
            //    return true;
            //}

            if (Map.TryGetValue(new Vector2Int(x, y), out cell))
                return true;
            else
                return false;
        }

        public bool TryGetCell(Vector2Int pos, out Cell cell)
        {
            //    if (Map[pos.x, pos.y] == null)
            //    {
            //        cell = null;
            //        return false;
            //    }
            //    else
            //    {
            //        cell = Map[pos.x, pos.y];
            //        return true;
            //    }

            if (Map.TryGetValue(pos, out cell))
                return true;
            else
                return false;
        }

        public void Render()
        {
            int x = 0;
            for (; x < Size.x; x++)
                for (int y = 0; y < Size.y; y++)
                {
                    Map.TryGetValue(new Vector2Int(x, y), out Cell c);
                    if (c.Terrain != null)
                    {
                        //UnityEngine.Debug.Log(c.Terrain);
                        RuleTile tile = c.Terrain.GetComponent<UnityTile>().Tile;
                        Terrain.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                }
        }

        public override string ToString() => $"{DisplayName} ({Position})";
    }
}

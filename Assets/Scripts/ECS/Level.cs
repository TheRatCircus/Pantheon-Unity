// Level.cs
// Jerome Martina

using Pantheon.ECS.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.ECS
{
    public sealed class Level : MonoBehaviour
    {
        [SerializeField] private Tilemap terrain = default;
        public Tilemap Terrain => terrain;

        public Dictionary<Vector2Int, Cell> Map { get; private set; }
            = new Dictionary<Vector2Int, Cell>();

        private void Awake()
        {
            for (int x = 0; x < 64; x++)
                for (int y = 0; y < 64; y++)
                {
                    Walkable walkable = new Walkable();
                    UnityTile tile = new UnityTile(Resources.Load<RuleTile>("Grass"));
                    Entity ground = new Entity(walkable, tile);

                    Cell cell = new Cell();
                    cell.AddEntity(ground);
                    Map.Add(new Vector2Int(x, y), cell);
                    UpdateCellVisibility(tile, x, y);
                }
        }

        private void UpdateCellVisibility(UnityTile tile, int x, int y)
        {
            terrain.SetTile(new Vector3Int(x, y, 0), tile.Tile);
        }
        
        private Cell TryGetCell(int x, int y)
        {
            if (!Map.TryGetValue(new Vector2Int(x, y), out Cell ret))
                return null;
            else
               return ret;
        }
    }
}

// Vault.cs
// Jerome Martina

#define DEBUG_VAULT
#undef DEBUG_VAULT

using Pantheon.Content;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.Gen
{
    /// <summary>
    /// A prefab of objects placed in the world at generation time.
    /// </summary>
    [CreateAssetMenu(fileName = "New Vault",
        menuName = "Pantheon/Content/Vault")]
    public sealed class Vault : ScriptableObject
    {
        [SerializeField] private GameObject prefab = null;
        private Tilemap tilemap = null;
        [SerializeField]
        private List<TerrainDefinition> terrain
            = new List<TerrainDefinition>();

        public Vector2Int Size => (Vector2Int)tilemap.size;

        public void Initialize()
        {
            tilemap = prefab.GetComponent<Tilemap>();
            tilemap.CompressBounds();
        }

        public TerrainDefinition GetTerrain(int x, int y)
        {
            TileBase marker = tilemap.GetTile
                ((Vector3Int)new Vector2Int(x, y));

            if (marker == null)
                return null;

            int markerIndex = int.Parse(marker.name.Split('_')[1]);
            return terrain[markerIndex];
        }

        public static bool Build(Vault vault, World.Level level,
            Vector2Int position)
        {
            vault.Initialize();

            if (!level.Contains(position + vault.Size))
            {
                UnityEngine.Debug.LogWarning(
                    "Position too close to level bounds.");
                return false;
            }

            for (int x = position.x; x < vault.Size.x + position.x; x++)
                for (int y = position.y; y < vault.Size.y + position.y; y++)
                {
                    TerrainDefinition terrain = vault.GetTerrain(
                        x - position.x, y - position.y);
                    if (terrain != null)
                    {
                        level.GetCell(x, y).Terrain = terrain;
                    }
                }
            return true;
        }
    }
}

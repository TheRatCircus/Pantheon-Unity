// Vault.cs
// Jerome Martina

#define DEBUG_VAULT
#undef DEBUG_VAULT

using Pantheon.Content;
using Pantheon.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.Gen
{
    enum VaultCellFlagEnum
    {
        Spawn
    }

    [Serializable]
    struct VaultCellFlag
    {
        public Vector2Int position;
        public VaultCellFlagEnum flag;
    }

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
        [SerializeField]
        private VaultCellFlag[] cellFlags = default;

        public Vector2Int Size => (Vector2Int)tilemap.size;

        public void Initialize()
        {
            tilemap = prefab.GetComponent<Tilemap>();
            tilemap.CompressBounds();
        }

        /// <summary>
        /// Returns the relative position of the spawn flag if this vault has one.
        /// </summary>
        public Vector2Int GetSpawnPosition()
        {
            if (cellFlags != null)
                foreach (VaultCellFlag vcf in cellFlags)
                    if (vcf.flag == VaultCellFlagEnum.Spawn)
                        return vcf.position;
            
            return new Vector2Int(-1, -1);
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

        /// <summary>
        /// Safe vault building function for internal use.
        /// </summary>
        /// <param name="position">Lower-left corner.</param>
        /// <param name="rotation">Rotation in degrees.</param>
        /// <returns>The vault definition so its flags can be consumed.</returns>
        public static Vault Build(
            string id, 
            World.Level level, 
            Vector2Int position,
            float rotation)
        {
            if (!Assets.Vaults.TryGetValue(id, out Vault vault))
                throw new ArgumentException($"Vault {id} not found.");

            vault.Initialize();

            //if (!level.Contains(position + vault.Size))
            //    throw new ArgumentException
            //        ("Position too close to level bounds.");

            for (int x = position.x; x < vault.Size.x + position.x; x++)
                for (int y = position.y; y < vault.Size.y + position.y; y++)
                {
                    TerrainDefinition terrain = vault.GetTerrain(
                        x - position.x, y - position.y);
                    if (terrain != null)
                    {
                        Vector2Int rot = new Vector2Int(
                            x - position.x,
                            y - position.y).Rotate(rotation);
                        if (level.TryGetCell(rot + position, out World.Cell cell))
                            cell.Terrain = terrain;
                    }
                }

            return vault;
        }

        /// <summary>
        /// Gracefully-failing vault build for use by console commands.
        /// </summary>
        /// <param name="position">Lower-left corner.</param>
        /// <param name="rotation">Rotation in degrees.</param>
        /// <returns>True if vault build was successful.</returns>
        public static bool TryBuild(
            string id,
            World.Level level,
            Vector2Int position,
            float rotation)
        {
            if (!Assets.Vaults.TryGetValue(id, out Vault vault))
                return false;

            vault.Initialize();

            if (!level.Contains(position + vault.Size))
                return false;

            for (int x = position.x; x < vault.Size.x + position.x; x++)
                for (int y = position.y; y < vault.Size.y + position.y; y++)
                {
                    TerrainDefinition terrain = vault.GetTerrain(
                        x - position.x, y - position.y);
                    if (terrain != null)
                    {
                        Vector2Int rot = new Vector2Int(
                            x - position.x,
                            y - position.y).Rotate(rotation);
                        if (level.TryGetCell(rot + position, out World.Cell cell))
                            cell.Terrain = terrain;
                    }
                }

            return true;
        }
    }
}

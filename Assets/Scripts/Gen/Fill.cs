// Fill.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    public sealed class Fill : BuilderStep
    {
        [JsonProperty] private TerrainDefinition terrain = default;

        /// <summary>
        /// ID-only constructor for JSON writing.
        /// </summary>
        /// <param name="terrainID"></param>
        public Fill(string terrainID)
        {
            terrain = ScriptableObject.CreateInstance<TerrainDefinition>();
            terrain.name = terrainID;
        }

        [JsonConstructor]
        public Fill(TerrainDefinition terrain)
        {
            this.terrain = terrain;
        }

        public override void Run(Level level)
        {
            int x = 0;
            for (; x < level.CellSize.x; x++)
                for (int y = 0; y < level.CellSize.y; y++)
                    if (level.TryGetCell(x, y, out Cell c))
                    {
                        c.Terrain = terrain;
                    }
        }
    }
}

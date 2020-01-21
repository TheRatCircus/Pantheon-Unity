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
        [JsonProperty] private TerrainDefinition ground = default;
        [JsonProperty] private TerrainDefinition wall = default;

        /// <summary>
        /// ID-only constructor for JSON writing.
        /// </summary>
        /// <param name="terrainID"></param>
        public Fill(string wallID, string groundID)
        {
            wall = ScriptableObject.CreateInstance<TerrainDefinition>();
            wall.name = wallID;
            ground = ScriptableObject.CreateInstance<TerrainDefinition>();
            ground.name = groundID;
        }

        [JsonConstructor]
        public Fill(TerrainDefinition wall, TerrainDefinition ground)
        {
            this.wall = wall;
            this.ground = ground;
        }

        public override void Run(Level level)
        {
            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                    if (level.TryGetCell(x, y, out Cell c))
                    {
                        c.Wall = wall;
                        c.Ground = ground;
                    }
        }
    }
}

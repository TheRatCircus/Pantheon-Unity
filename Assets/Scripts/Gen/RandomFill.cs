// RandomFill.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    public sealed class RandomFill : BuilderStep
    {
        [JsonProperty] private int percent = 50; // 0...100
        [JsonProperty] private TerrainDefinition terrain = default;

        /// <summary>
        /// ID-only constructor for JSON writing.
        /// </summary>
        /// <param name="terrainID"></param>
        public RandomFill(string terrainID, int percent)
        {
            terrain = ScriptableObject.CreateInstance<TerrainDefinition>();
            terrain.name = terrainID;
            this.percent = percent;
        }

        [JsonConstructor]
        public RandomFill(TerrainDefinition terrain, int percent)
        {
            this.terrain = terrain;
            this.percent = percent;
        }

        public override void Run(Level level)
        {
            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                {
                    if (Random.Range(0, 100) < percent)
                        if (level.TryGetCell(x, y, out Cell c))
                            c.Terrain = terrain;
                }
        }

        public override string ToString()
        {
            return $"Random fill {terrain} at %{percent}";
        }
    }
}

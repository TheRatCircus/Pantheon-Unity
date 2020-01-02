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
        [JsonProperty] private TerrainDefinition ground = default;
        [JsonProperty] private TerrainDefinition wall = default;

        /// <summary>
        /// ID-only constructor for JSON writing.
        /// </summary>
        /// <param name="wallID"></param>
        public RandomFill(string wallID, string groundID, int percent)
        {
            wall = ScriptableObject.CreateInstance<TerrainDefinition>();
            wall.name = wallID;
            ground = ScriptableObject.CreateInstance<TerrainDefinition>();
            ground.name = groundID;
            this.percent = percent;
        }

        [JsonConstructor]
        public RandomFill(TerrainDefinition wall, TerrainDefinition ground, int percent)
        {
            this.wall = wall;
            this.ground = ground;
            this.percent = percent;
        }

        public override void Run(Level level)
        {
            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                    if (Random.Range(0, 100) < percent)
                        if (level.TryGetCell(x, y, out Cell c))
                        {
                            c.Wall = wall;
                            c.Ground = ground;
                        }
        }

        public override string ToString()
        {
            return $"Random fill {wall} at %{percent}";
        }
    }
}

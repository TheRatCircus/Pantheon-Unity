// RandomFill.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    [CreateAssetMenu(fileName = "New Random Fill Step",
        menuName = "Pantheon/Builder Step/Random Fill")]
    public sealed class RandomFill : BuilderStep
    {
        [JsonProperty] [SerializeField] private int percent = 50; // 0...100
        [JsonProperty] [SerializeField] private TerrainDefinition terrain = default;

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

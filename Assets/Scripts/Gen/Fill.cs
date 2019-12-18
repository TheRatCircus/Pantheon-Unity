// Fill.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    [CreateAssetMenu(fileName = "New Fill Step",
        menuName = "Pantheon/Builder Step/Fill")]
    public sealed class Fill : BuilderStep
    {
        [JsonProperty] [SerializeField]
        private TerrainDefinition terrain = default;

        public override void Run(Level level)
        {
            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                {
                    if (level.TryGetCell(x, y, out Cell c))
                    {
                        c.Terrain = terrain;
                    }
                }
        }
    }
}

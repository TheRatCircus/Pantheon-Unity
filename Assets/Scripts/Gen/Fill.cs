// Fill.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.ECS;
using Pantheon.ECS.Templates;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    [System.Serializable]
    public sealed class Fill : BuilderStep
    {
        [JsonProperty] private string fillEntityID;

        public Fill(string fillEntityID)
        {
            this.fillEntityID = fillEntityID;
        }

        public override void Run(Level level, EntityFactory factory)
        {
            AssetLoader loader = new AssetLoader();
            Template template = loader.Load<Template>(fillEntityID);

            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                    if (level.TryGetCell(x, y, out Cell c))
                        factory.NewEntityAt(template, level, c);

            loader.Unload(false);
        }
    }

    [System.Serializable]
    public sealed class RandomFill : BuilderStep
    {
        [JsonProperty] private int percent; // 0...100
        [JsonProperty] private string fillEntityID;

        public RandomFill(int percent, string fillEntityID)
        {
            this.percent = percent;
            this.fillEntityID = fillEntityID;
        }

        public override void Run(Level level, EntityFactory factory)
        {
            AssetLoader loader = new AssetLoader();
            Template template = loader.Load<Template>(fillEntityID);

            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                {
                    if (Random.Range(0, 100) < percent)
                        if (level.TryGetCell(x, y, out Cell c))
                            factory.NewEntityAt(template, level, c);
                }

            loader.Unload(false);
        }

        public override string ToString()
        {
            return $"Random fill with {fillEntityID} at %{percent}";
        }
    }
}

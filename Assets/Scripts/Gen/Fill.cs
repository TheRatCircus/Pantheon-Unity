// Fill.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.ECS;
using Pantheon.ECS.Templates;
using Pantheon.World;
using System;
using Random = UnityEngine.Random;

namespace Pantheon.Gen
{
    [Serializable]
    public sealed class Fill : BuilderStep
    {
        [JsonProperty] private string fillEntityID;

        public Fill(string fillEntityID)
        {
            this.fillEntityID = fillEntityID;
        }

        public override void Run(Level level, AssetLoader loader,
            EntityFactory factory)
        {
            Template template = loader.Load<Template>(fillEntityID);

            if (!level.AssetIDCache.Contains(fillEntityID))
                    level.AssetIDCache.Add(fillEntityID);

            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                    if (level.TryGetCell(x, y, out Cell c))
                        factory.FlyweightEntityAt(template, level, c);
        }
    }

    [Serializable]
    public sealed class RandomFill : BuilderStep
    {
        [JsonProperty] private int percent; // 0...100
        [JsonProperty] private string fillEntityID;

        public RandomFill(int percent, string fillEntityID)
        {
            this.percent = percent;
            this.fillEntityID = fillEntityID;
        }

        public override void Run(Level level, AssetLoader loader,
            EntityFactory factory)
        {
            Template template = loader.Load<Template>(fillEntityID);

            if (!level.AssetIDCache.Contains(fillEntityID))
                level.AssetIDCache.Add(fillEntityID);

            int x = 0;
            for (; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                {
                    if (Random.Range(0, 100) < percent)
                        if (level.TryGetCell(x, y, out Cell c))
                            factory.FlyweightEntityAt(template, level, c);
                }
        }

        public override string ToString()
        {
            return $"Random fill with {fillEntityID} at %{percent}";
        }
    }
}

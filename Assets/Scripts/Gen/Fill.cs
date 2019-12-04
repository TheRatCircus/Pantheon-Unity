// Fill.cs
// Jerome Martina

using Newtonsoft.Json;
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

        public override void Run(Level level, AssetLoader loader)
        {

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

        public override void Run(Level level, AssetLoader loader)
        {

        }

        public override string ToString()
        {
            return $"Random fill with {fillEntityID} at %{percent}";
        }
    }
}

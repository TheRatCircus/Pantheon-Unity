// BuilderPlan.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.ECS;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    /// <summary>
    /// An invocable set of instructions for procedurally generating levels.
    /// </summary>
    [System.Serializable]
    public sealed class BuilderPlan
    {
        [JsonProperty] private BuilderStep[] steps;

        public BuilderPlan(params BuilderStep[] steps)
        {
            this.steps = steps;
        }

        public static BuilderPlan Load(string name)
        {
            AssetLoader loader = new AssetLoader();
            TextAsset json = loader.Load<TextAsset>(name);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.All;
            BuilderPlan plan = JsonConvert.DeserializeObject<BuilderPlan>(
                json.text, settings);
            loader.Unload(true);
            return plan;
        }

        public void Run(Level level, int sizeX, int sizeY, EntityFactory factory)
        {
            InitializeMap(level, sizeX, sizeY);
            foreach (BuilderStep step in steps)
                step.Run(level, factory);
        }

        /// <summary>
        /// Kick off by populating the level with cells.
        /// </summary>
        private void InitializeMap(Level level, int sizeX, int sizeY)
        {
            level.Size = new Vector2Int(sizeX, sizeY);

            int x = 0;
            for (; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                {
                    Cell cell = new Cell(new Vector2Int(x, y));
                    level.Map.Add(cell.Position, cell);
                }
        }

        public override string ToString()
        {
            string ret = "";
            foreach (BuilderStep step in steps)
                ret += step;
            return ret;
        }
    }
}

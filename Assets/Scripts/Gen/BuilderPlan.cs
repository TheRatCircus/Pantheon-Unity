// BuilderPlan.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    /// <summary>
    /// An invocable set of instructions for procedurally generating levels.
    /// </summary>
    [CreateAssetMenu(fileName = "New Builder Plan",
        menuName = "Pantheon/Builder Plan")]
    public sealed class BuilderPlan : ScriptableObject
    {
        [JsonProperty] [SerializeField] private BuilderStep[] steps;

        public static BuilderPlan NewBuilderPlan(params BuilderStep[] steps)
        {
            BuilderPlan plan = CreateInstance<BuilderPlan>();
            plan.steps = steps;
            return plan;
        }

        public void Run(Level level, int sizeX, int sizeY)
        {
            InitializeMap(level, sizeX, sizeY);
            foreach (BuilderStep step in steps)
                step.Run(level);
        }

        /// <summary>
        /// Kick off by populating the level with cells.
        /// </summary>
        private void InitializeMap(Level level, int sizeX, int sizeY)
        {
            level.Size = new Vector2Int(sizeX, sizeY);
            level.Map = new Cell[sizeX, sizeY];

            int x = 0;
            for (; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    level.Map[x, y] = new Cell(new Vector2Int(x, y));
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

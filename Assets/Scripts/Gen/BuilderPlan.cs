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
        public BuilderStep[] Steps => steps;

        public static BuilderPlan NewBuilderPlan(params BuilderStep[] steps)
        {
            BuilderPlan plan = CreateInstance<BuilderPlan>();
            plan.steps = steps;
            return plan;
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

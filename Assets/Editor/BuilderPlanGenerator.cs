// BuilderPlanGenerator.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Gen;
using UnityEditor;

namespace PantheonEditor
{
    public static class BuilderPlanGenerator
    {
        [MenuItem("Assets/Pantheon/Generate Builder Plan")]
        private static void GenerateBuilderPlan()
        {
            BuilderPlan plan = new BuilderPlan(new BuilderStep[]
                {
                    new Fill("Ground_Grass"),
                    new RandomFill(50, "Wall_StoneWall")
                });
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Arrays;
            string json = JsonConvert.SerializeObject(plan, settings);
            UnityEngine.Debug.Log(json);
        }
    }
}

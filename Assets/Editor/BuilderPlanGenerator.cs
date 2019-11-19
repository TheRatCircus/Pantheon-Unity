// BuilderPlanGenerator.cs
// Jerome Martina

using Pantheon;
using Newtonsoft.Json;
using Pantheon.Gen;
using UnityEditor;
using Pantheon.ECS.Components;
using Pantheon.ECS.Templates;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

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

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Serialization._builderStepBinder,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(plan, settings);

            UnityEngine.Debug.Log(json);
        }

        [MenuItem("Assets/Pantheon/Serialize Entity Test")]
        private static void SerializeEntityTest()
        {
            EntityTemplate input = new EntityTemplate()
            {
                Components = new Dictionary<string, BaseComponent>()
                {
                    { "Health", new Health(20, 1500) }
                }
            };

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Serialization._entityBinder,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(input, settings);
            UnityEngine.Debug.Log(json);
        }
    }
}

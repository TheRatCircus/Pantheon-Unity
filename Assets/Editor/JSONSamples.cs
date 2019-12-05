// JSONSamples.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Gen;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    /// <summary>
    /// Editor functions to generate sample JSON text as reference material.
    /// </summary>
    internal static class JSONSamples
    {
        [MenuItem("Assets/Pantheon/JSON Samples/Builder Plan")]
        static void SampleBuilderPlan()
        {
            BuilderPlan plan = BuilderPlan.NewBuilderPlan();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Serialization._builderStepBinder,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(plan, settings);
            File.WriteAllText(Path.Combine(
                Application.dataPath, "/sample_plan.json"), json);
        }
    }
}

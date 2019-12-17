// JSONSchemaGenerator.cs
// Jerome Martina

using UnityEngine;
using UnityEditor;
using Pantheon.Commands;
//using NJsonSchema;

namespace PantheonEditor
{
    /// <summary>
    /// Functions for generating JSON schema via NJsonSchema from Pantheon's classes.
    /// </summary>
    internal static class JSONSchemaGenerator
    {
        [MenuItem("Tools/Pantheon/JSON Schema From/ExplodeCommand")]
        private static void DoIt()
        {
            //JsonSchema schema = JsonSchema.FromType<ExplodeCommand>();
            //string schemaData = schema.ToJson();
            //Debug.Log(schema);
        }
    }
}

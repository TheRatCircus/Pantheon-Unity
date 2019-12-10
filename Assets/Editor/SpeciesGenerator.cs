// SpeciesGenerator.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pantheon;
using Newtonsoft.Json;
using Pantheon.Serialization.Json.Converters;
using System.IO;
using BodyPart = Pantheon.BodyPart;

namespace PantheonEditor
{
    internal sealed class SpeciesGenerator : EditorWindow
    {
        [MenuItem("Tools/Pantheon/Species Generator")]
        public static void Open()
        {
            SpeciesGenerator editor = GetWindow<SpeciesGenerator>();
        }

        private static readonly JsonSerializerSettings jsonSettings
            = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(),
                    new BodyPartConverter()
                }
            };

        public SpeciesDefinition def;
        public string[] partIDs = default;

        private void OnGUI()
        {
            SerializedObject obj = new SerializedObject(this);

            EditorGUILayout.PropertyField(obj.FindProperty("def"),
                new GUIContent("Species"), true);
            EditorGUILayout.PropertyField(obj.FindProperty("partIDs"),
                new GUIContent("Parts"), true);

            obj.ApplyModifiedProperties();

            if (GUILayout.Button("Serialize"))
                Serialize();
        }

        private void Serialize()
        {
            BodyPart[] parts = new BodyPart[partIDs.Length];

            for (int i = 0; i < partIDs.Length; i++)
                parts[i] = new BodyPart(partIDs[i]);

            def = new SpeciesDefinition(def.ID, def.Name, def.Sprite, parts);

            string json = JsonConvert.SerializeObject(def, jsonSettings);
            string path = Application.dataPath + $"/Content/Species/{def.ID}.json";
            File.WriteAllText(path, json);
            Debug.Log($"Wrote species definition to {path}.");
        }
    }
}

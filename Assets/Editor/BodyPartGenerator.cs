// BodyPartGenerator.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using BodyPart = Pantheon.BodyPart;

namespace PantheonEditor
{
    internal sealed class BodyPartGenerator : EditorWindow
    {
        [MenuItem("Tools/Pantheon/Body Part Generator")]
        public static void Open()
        {
            BodyPartGenerator editor = GetWindow<BodyPartGenerator>();
        }

        private static readonly JsonSerializerSettings jsonSettings
            = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._entityBinder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(),
                    new RuleTileConverter()
                }
            };

        public BodyPart part;

        private void Awake()
        {
            part = new BodyPart();
        }

        private void OnGUI()
        {
            SerializedObject obj = new SerializedObject(this);
            SerializedProperty prop = obj.FindProperty("part");
            EditorGUILayout.PropertyField(prop, new GUIContent("Part"), true);
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Serialize"))
                Serialize(obj);
        }

        private void Serialize(SerializedObject obj)
        {
            string json = JsonConvert.SerializeObject(part, jsonSettings);
            string path = Application.dataPath + $"/Content/BodyParts/{part.ID}.json";
            File.WriteAllText(path, json);
            Debug.Log($"Wrote body part definition to {path}.");
        }
    }
}

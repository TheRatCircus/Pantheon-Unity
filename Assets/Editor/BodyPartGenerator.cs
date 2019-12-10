// BodyPartGenerator.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
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
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Serialization._builderStepBinder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(),
                    new RuleTileConverter()
                }
            };

            string json = JsonConvert.SerializeObject(part, settings);
            string path = Application.dataPath + $"/Content/BodyParts/{part.ID}.json";
            File.WriteAllText(path, json);
            Debug.Log($"Wrote body part definition to {path}.");
        }
    }
}

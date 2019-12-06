// TemplateEditor.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Components;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    internal sealed class TemplateEditor : EditorWindow
    {
        [MenuItem("Tools/Template Editor")]
        public static void Open()
        {
            TemplateEditor editor = GetWindow<TemplateEditor>();
        }

        public string templateName = "Template Name";
        public Sprite sprite = default;

        public Actor actor;
        public AI ai;

        private EntityComponent[] components;
        private bool[] componentsIncluded = new bool[2];

        private void Awake()
        {
            actor = new Actor();
            ai = new AI();

            components = new EntityComponent[]
            {
                actor,
                ai
            };
        }

        private void OnGUI()
        {
            SerializedObject obj = new SerializedObject(this);

            componentsIncluded[0] = EditorGUILayout.Toggle("Actor", componentsIncluded[0]);
            componentsIncluded[1] = EditorGUILayout.Toggle("AI", componentsIncluded[1]);

            EditorGUILayout.PropertyField(obj.FindProperty("templateName"));
            EditorGUILayout.PropertyField(obj.FindProperty("sprite"));

            if (componentsIncluded[0])
                EditorGUILayout.PropertyField(obj.FindProperty("actor"), new GUIContent("Actor"), true);
            if (componentsIncluded[1])
                EditorGUILayout.PropertyField(obj.FindProperty("ai"), new GUIContent("AI"), true);

            obj.ApplyModifiedProperties();

            if (GUILayout.Button("Serialize"))
            {
                Serialize();
            }
        }

        private void Serialize()
        {
            SpriteConverter spriteConverter = new SpriteConverter();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Serialization._builderStepBinder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    spriteConverter
                }
            };

            List<EntityComponent> comps = new List<EntityComponent>();
            for (int i = 0; i < componentsIncluded.Length; i++)
            {
                if (componentsIncluded[i])
                    comps.Add(components[i]);
            }

            EntityTemplate template = new EntityTemplate(comps.ToArray())
            {
                Name = templateName,
                Sprite = sprite
            };

            string json = JsonConvert.SerializeObject(template, settings);
            File.WriteAllText(Application.dataPath + $"/{name}.json", json);
        }
    }
}

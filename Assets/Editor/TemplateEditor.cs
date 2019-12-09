// TemplateEditor.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon;
using Pantheon.Components;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        public string templateID = "Template ID";
        public Sprite sprite = default;
        public RuleTile tile = default;

        public Actor actor = new Actor();
        public AI ai = new AI();
        public Health health = new Health();

        private EntityComponent[] components;
        private bool[] componentsIncluded = new bool[3];

        private void Awake()
        {
            components = new EntityComponent[]
            {
                actor,
                ai,
                health
            };
        }

        private void OnGUI()
        {
            SerializedObject obj = new SerializedObject(this);

            componentsIncluded[0] = EditorGUILayout.Toggle("Actor", componentsIncluded[0]);
            componentsIncluded[1] = EditorGUILayout.Toggle("AI", componentsIncluded[1]);
            componentsIncluded[2] = EditorGUILayout.Toggle("Health", componentsIncluded[2]);

            EditorGUILayout.PropertyField(obj.FindProperty("templateID"));
            EditorGUILayout.PropertyField(obj.FindProperty("templateName"));
            EditorGUILayout.PropertyField(obj.FindProperty("sprite"));
            EditorGUILayout.PropertyField(obj.FindProperty("tile"));

            if (componentsIncluded[0])
                EditorGUILayout.PropertyField(obj.FindProperty("actor"), new GUIContent("Actor"), true);
            if (componentsIncluded[1])
                EditorGUILayout.PropertyField(obj.FindProperty("ai"), new GUIContent("AI"), true);
            if (componentsIncluded[2])
                EditorGUILayout.PropertyField(obj.FindProperty("health"), new GUIContent("Health"), true);

            obj.ApplyModifiedProperties();

            if (GUILayout.Button("Serialize"))
            {
                Serialize();
            }
        }

        private void Serialize()
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

            List<EntityComponent> comps = new List<EntityComponent>();
            for (int i = 0; i < componentsIncluded.Length; i++)
            {
                if (componentsIncluded[i])
                    comps.Add(components[i]);
            }

            EntityTemplate template = new EntityTemplate(
                templateID, templateName, sprite, comps.ToArray());

            string json = JsonConvert.SerializeObject(template, settings);
            string path = Application.dataPath + $"/Content/Templates/{templateID}.json";
            File.WriteAllText(path, json);
            Debug.Log($"Wrote template to {path}.");
        }
    }
}

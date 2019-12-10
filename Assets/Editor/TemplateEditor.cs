// TemplateEditor.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pantheon;
using Pantheon.Components;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    internal sealed class TemplateEditor : EditorWindow
    {
        [MenuItem("Tools/Pantheon/Template Editor")]
        public static void Open()
        {
            TemplateEditor editor = GetWindow<TemplateEditor>();
        }

        private static readonly JsonSerializerSettings jsonSettings
            = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            SerializationBinder = Binders._entityBinder,
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter>()
            {
                new SpriteConverter(),
                new RuleTileConverter(),
                new SpeciesDefinitionConverter()
            }
        };

        private SerializedObject obj;

        public TextAsset jsonFile = default;
        public JObject template;

        // Components
        public Actor actor = default;

        public Species species = default;
        public string speciesID;

        public EntityComponent[] components;
        private int selectedComponent = -1;

        private void Awake()
        {
            obj = new SerializedObject(this);
            components = new EntityComponent[]
            {
                actor,
                species
            };
        }

        private void OnGUI()
        {
            SerializedProperty prop = obj.FindProperty("jsonFile");
            EditorGUILayout.PropertyField(prop, new GUIContent("Template File"));
            obj.ApplyModifiedProperties();

            if (prop.objectReferenceValue == null)
                return;

            if (GUILayout.Button("Select Component to Add"))
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < components.Length; i++)
                {
                    menu.AddItem(
                        new GUIContent(components[i].GetType().ToString()),
                        selectedComponent == i,
                        SelectComponent, i);
                }
                menu.ShowAsContext();
            }

            switch (selectedComponent)
            {
                case 0:
                    EditorGUILayout.PropertyField(obj.FindProperty("actor"),
                        new GUIContent("Actor"), true);
                    break;
                case 1:
                    speciesID = EditorGUILayout.TextField("Species", speciesID);
                    components[selectedComponent] = new Species(new SpeciesDefinition(speciesID));
                    break;
                default:
                    return;
            }
            obj.ApplyModifiedProperties();

            if (GUILayout.Button("Add Component"))
            {
                template = JObject.Parse(jsonFile.text);
                JArray jComponents = (JArray)template["Components"];

                string json = JsonConvert.SerializeObject(
                    components[selectedComponent], jsonSettings);
                jComponents.Add(JToken.Parse(json));

                string path = AssetDatabase.GetAssetPath(jsonFile);
                File.WriteAllText(path, template.ToString());
                Debug.Log($"Added {components[selectedComponent]} to {jsonFile.name}.");
            }
        }

        private void SelectComponent(object i)
        {
            selectedComponent = (int)i;
        }
    }
}

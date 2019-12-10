// TemplateEditor.cs
// Jerome Martina

using Newtonsoft.Json.Linq;
using Pantheon.Components;
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

        private SerializedObject obj;

        public TextAsset jsonFile;
        public JObject template;

        public EntityComponent[] components = new EntityComponent[]
            {
                new Actor(),
                new AI(),
                new Health(),
                new Melee()
            };
        int selectedComponent = -1;

        private void Awake()
        {
            obj = new SerializedObject(this);
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
                for (int i = 0; i < this.components.Length; i++)
                {
                    menu.AddItem(
                        new GUIContent(this.components[i].GetType().ToString()),
                        selectedComponent == i,
                        SelectComponent, i);
                }
                menu.ShowAsContext();
            }

            template = JObject.Parse(jsonFile.text);
            JArray components = (JArray)template["Components"];
            //components.Add(JToken.FromObject(component));
        }

        private void SelectComponent(object i)
        {
            selectedComponent = (int)i;
        }
    }
}

// LevelGenTestEditor.cs
// Jerome Martina

using Pantheon;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    [CustomEditor(typeof(LevelGenTester))]
    public sealed class LevelGenTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LevelGenTester tester = target as LevelGenTester;

            if (GUILayout.Button("Run Plan"))
                tester.RunPlan();
            if (GUILayout.Button("Clear"))
                tester.Clear();
        }
    }
}

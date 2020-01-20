// LevelWrapperEditor.cs
// Jerome Martina

using Pantheon;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    [CustomEditor(typeof(LevelWrapper))]
    public sealed class LevelWrapperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
        }

        private void OnSceneGUI()
        {
            LevelWrapper wrapper = target as LevelWrapper;

            Handles.BeginGUI();
            for (int x = 0; x < wrapper.Level.FleeMap.Map.GetLength(0); x++)
                for (int y = 0; y < wrapper.Level.FleeMap.Map.GetLength(1); y++)
                {
                    if (wrapper.Level.FleeMap.Map[x, y] >= 255 ||
                        wrapper.Level.FleeMap.Map[x, y] <= -30)
                        continue;

                    Handles.Label(new Vector3(x, y),
                        wrapper.Level.FleeMap.Map[x, y].ToString());
                }
            Handles.EndGUI();
        }
    }
}
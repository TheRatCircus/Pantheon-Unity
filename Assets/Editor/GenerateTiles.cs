// GenerateTiles.cs
// Jerome Martina

using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PantheonEditor
{
    public static class GenerateTiles
    {
        /// <summary>
        /// Automatically generate tiles from sprites for actors and items.
        /// </summary>
        [MenuItem("Assets/Pantheon/Generate Tiles")]
        private static void AutogenerateTiles()
        {
            string[] actorSpriteGuids = AssetDatabase.FindAssets("t:sprite",
                new[] { "Assets/Sprites/Actor" });
            string[] itemSpriteGuids = AssetDatabase.FindAssets("t:sprite",
                new[] { "Assets/Sprites/Item" });

            foreach (string guid in actorSpriteGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                RuleTile tile = ScriptableObject.CreateInstance<RuleTile>();
                tile.m_DefaultSprite = sprite;
                string name = sprite.name.Split('_')[1];
                AssetDatabase.CreateAsset(tile, $"Assets/Tiles/Actor/Tile_{name}.asset");
            }

            foreach (string guid in itemSpriteGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = sprite;
                tile.flags = TileFlags.LockTransform;
                string name = sprite.name.Split('_')[1];
                AssetDatabase.CreateAsset(tile, $"Assets/Tiles/Item/Tile_{name}.asset");
            }

            Debug.Log("Finished auto-generating tiles.");
        }
    }
}

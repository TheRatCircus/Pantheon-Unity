// PopulateDatabase.cs
// Jerome Martina

using Pantheon;
using Pantheon.Core;
using UnityEditor;
using UnityEngine;

namespace PantheonEditor
{
    public static class Database
    {
        public static readonly string ItemsPath = "Assets/Content/Items";

        /// <summary>
        /// Fill the in-game content database with all relevant objects.
        /// </summary>
        [MenuItem("Assets/Pantheon/Populate Database")]
        private static void PopulateDatabase()
        {
            GameObject gameControllerObj = GameObject.Find("GameController");
            Game gameController = gameControllerObj.GetComponent<Game>();

            Pantheon.Core.Database db = gameController.Database;
            db.ItemList.Clear();

            string[] itemGuids = AssetDatabase.FindAssets(
                "l:item", new string[] { ItemsPath });

            foreach (string guid in itemGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"Adding {assetPath}...");
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(
                    assetPath);

                db.ItemList.Add(item);
            }

            Debug.Log("Finished populating database.");
        }
    }
}

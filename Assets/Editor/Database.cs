// PopulateDatabase.cs
// Jerome Martina

using Pantheon;
using Pantheon.Core;
using Pantheon.WorldGen;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PantheonEditor
{
    public static class Database
    {
        /// <summary>
        /// Fill the in-game content database with all relevant objects.
        /// </summary>
        [MenuItem("Assets/Pantheon/Populate Database")]
        private static void PopulateDatabase()
        {
            GameObject gameControllerObj = GameObject.Find("GameController");
            Game gameController = gameControllerObj.GetComponent<Game>();

            Pantheon.Core.Database db = gameController.Database;

            PopulateDatabaseItems(db);
            PopulateDatabaseTerrains(db);
            PopulateDatabaseNPCs(db);
            PopulateDatabaseFeatures(db);
            PopulateDatabaseSpells(db);
            PopulateDatabaseAspects(db);
            PopulateDatabaseSpecies(db);
            PopulateDatabaseOccupations(db);
            PopulateDatabaseLandmarks(db);

            EditorSceneManager.SaveScene(db.gameObject.scene);
            Debug.Log("Finished populating database.");
        }

        private static void PopulateDatabaseItems(Pantheon.Core.Database db)
        {
            string[] itemGuids = AssetDatabase.FindAssets(
                "t:ItemDef", new string[] { "Assets/Content/Items" });

            db.ItemList.Clear();
            db.ItemList.Capacity = itemGuids.Length;

            for (int i = 0; i < itemGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(itemGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                ItemDef item = AssetDatabase.LoadAssetAtPath<ItemDef>(
                    assetPath);

                if (item.ID == ItemID.Default)
                    Debug.LogWarning($"{item.name} has no ID.");

                db.ItemList.Add(item);
            }
        }

        private static void PopulateDatabaseTerrains(Pantheon.Core.Database db)
        {
            string[] terrainGuids = AssetDatabase.FindAssets(
                "t:TerrainDef", new string[] { "Assets/Content/Terrain" });

            db.TerrainList.Clear();
            db.TerrainList.Capacity = terrainGuids.Length;

            for (int i = 0; i < terrainGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(terrainGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                TerrainDef terrain = AssetDatabase.LoadAssetAtPath<TerrainDef>(
                    assetPath);

                if (terrain.ID == TerrainID.Default)
                    Debug.LogWarning($"{terrain.name} has no ID.");

                db.TerrainList.Add(terrain);
            }
        }

        private static void PopulateDatabaseNPCs(Pantheon.Core.Database db)
        {
            string[] npcGuids = AssetDatabase.FindAssets(
                "t:NPCWrapper", new string[] { "Assets/Content/NPCs" });

            db.NPCList.Clear();
            db.NPCList.Capacity = npcGuids.Length;

            for (int i = 0; i < npcGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(npcGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                NPCWrapper npc = AssetDatabase.LoadAssetAtPath<NPCWrapper>(
                    assetPath);

                if (npc.ID == NPCID.Default)
                    Debug.LogWarning($"{npc.name} has no ID.");

                db.NPCList.Add(npc);
            }
        }

        private static void PopulateDatabaseFeatures(Pantheon.Core.Database db)
        {
            string[] featureGuids = AssetDatabase.FindAssets(
                "t:FeatureDef", new string[] { "Assets/Content/Features" });

            db.FeatureList.Clear();
            db.FeatureList.Capacity = featureGuids.Length;

            for (int i = 0; i < featureGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(featureGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                FeatureDef feat = AssetDatabase.LoadAssetAtPath<FeatureDef>(
                    assetPath);

                if (feat.ID == FeatureID.Default)
                    Debug.LogWarning($"{feat.name} has no ID.");

                db.FeatureList.Add(feat);
            }
        }

        private static void PopulateDatabaseSpells(Pantheon.Core.Database db)
        {
            string[] spellGuids = AssetDatabase.FindAssets(
                "t:Spell", new string[] { "Assets/Content/Spells" });

            db.SpellList.Clear();
            db.SpellList.Capacity = spellGuids.Length;

            for (int i = 0; i < spellGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(spellGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Spell spell = AssetDatabase.LoadAssetAtPath<Spell>(
                    assetPath);

                if (spell.ID == SpellID.Default)
                    Debug.LogWarning($"{spell.name} has no ID.");

                db.SpellList.Add(spell);
            }
        }

        private static void PopulateDatabaseAspects(Pantheon.Core.Database db)
        {
            string[] aspectGuids = AssetDatabase.FindAssets(
                "t:Aspect", new string[] { "Assets/Content/Aspects" });

            db.AspectList.Clear();
            db.AspectList.Capacity = aspectGuids.Length;

            for (int i = 0; i < aspectGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(aspectGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Aspect aspect = AssetDatabase.LoadAssetAtPath<Aspect>(
                    assetPath);

                if (aspect.ID == AspectID.Default)
                    Debug.LogWarning($"{aspect.name} has no ID.");

                db.AspectList.Add(aspect);
            }
        }

        private static void PopulateDatabaseSpecies(Pantheon.Core.Database db)
        {
            string[] speciesGuids = AssetDatabase.FindAssets(
                "t:Species", new string[] { "Assets/Content/Species" });

            db.SpeciesList.Clear();
            db.SpeciesList.Capacity = speciesGuids.Length;

            for (int i = 0; i < speciesGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(speciesGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Species species = AssetDatabase.LoadAssetAtPath<Species>(
                    assetPath);

                if (species.ID == SpeciesID.Default)
                    Debug.LogWarning($"{species.name} has no ID.");

                db.SpeciesList.Add(species);
            }
        }

        private static void PopulateDatabaseOccupations(Pantheon.Core.Database db)
        {
            string[] occGuids = AssetDatabase.FindAssets(
                "t:Occupation", new string[] { "Assets/Content/Occupations" });

            db.OccupationList.Clear();
            db.OccupationList.Capacity = occGuids.Length;

            for (int i = 0; i < occGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(occGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Occupation occ = AssetDatabase.LoadAssetAtPath<Occupation>(
                    assetPath);

                if (occ.ID == OccupationID.Default)
                    Debug.LogWarning($"{occ.name} has no ID.");

                db.OccupationList.Add(occ);
            }
        }

        private static void PopulateDatabaseLandmarks(Pantheon.Core.Database db)
        {
            string[] landmarkGuids = AssetDatabase.FindAssets(
                "t:Landmark", new string[] { "Assets/Prefabs/Landmarks" });

            db.LandmarkList.Clear();
            db.LandmarkList.Capacity = landmarkGuids.Length;

            for (int i = 0; i < landmarkGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(landmarkGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Landmark landmark = AssetDatabase.LoadAssetAtPath<Landmark>(
                    assetPath);

                if (landmark.ID == LandmarkID.Default)
                    Debug.LogWarning($"{landmark.name} has no ID.");

                db.LandmarkList.Add(landmark);
            }
        }
    }
}

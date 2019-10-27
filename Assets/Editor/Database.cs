// Database.cs
// Jerome Martina

using Pantheon;
using Pantheon.Core;
using Pantheon.WorldGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PantheonEditor
{
    public static class Database
    {
        private static List<FieldInfo> GetConstants(System.Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(
                BindingFlags.Public |
                BindingFlags.Static);

            return fieldInfos.Where(
                f => f.FieldType == typeof(string) &&
                f.IsInitOnly)
                .ToList();
        }

        /// <summary>
        /// Fill the in-game content database with all relevant objects.
        /// </summary>
        [MenuItem("Assets/Pantheon/Populate Database")]
        private static void PopulateDatabase()
        {
            GameObject gameControllerObj = GameObject.Find("GameController");
            Game gameController = gameControllerObj.GetComponent<Game>();

            Pantheon.Core.Database db = gameController.Database;

            db.Content.Clear();

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
            List<FieldInfo> itemConsts = GetConstants(typeof(ID.Item));

            string[] itemGuids = AssetDatabase.FindAssets(
                "t:ItemDef", new string[] { "Assets/Content/Items" });

            for (int i = 0; i < itemGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(itemGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                ItemDef item = AssetDatabase.LoadAssetAtPath<ItemDef>(
                    assetPath);

                item.SetID();

                string s = ""; // Dummy string for FieldInfo.GetValue()
                bool set = false;
                foreach (FieldInfo info in itemConsts)
                {
                    if (item.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(item);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{item.name} has no constant.");
            }
        }

        private static void PopulateDatabaseTerrains(Pantheon.Core.Database db)
        {
            List<FieldInfo> terrainConsts = GetConstants(typeof(ID.Terrain));

            string[] terrainGuids = AssetDatabase.FindAssets(
                "t:TerrainDef", new string[] { "Assets/Content/Terrain" });

            for (int i = 0; i < terrainGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(terrainGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                TerrainDef terrain = AssetDatabase.LoadAssetAtPath<TerrainDef>(
                    assetPath);

                terrain.SetID();

                string s = "";
                bool set = false;
                foreach (FieldInfo info in terrainConsts)
                {
                    if (terrain.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(terrain);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{terrain.name} has no constant.");
            }
        }

        private static void PopulateDatabaseNPCs(Pantheon.Core.Database db)
        {
            List<FieldInfo> npcConsts = GetConstants(typeof(ID.NPC));

            string[] npcGuids = AssetDatabase.FindAssets(
                "t:NPCWrapper", new string[] { "Assets/Content/NPCs" });

            for (int i = 0; i < npcGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(npcGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                NPCWrapper npc = AssetDatabase.LoadAssetAtPath<NPCWrapper>(
                    assetPath);

                npc.SetID();

                string s = "";
                bool set = false;
                foreach (FieldInfo info in npcConsts)
                {
                    if (npc.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(npc);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{npc.name} has no constant.");
            }
        }

        private static void PopulateDatabaseFeatures(Pantheon.Core.Database db)
        {
            List<FieldInfo> featConsts = GetConstants(typeof(ID.Feature));

            string[] featureGuids = AssetDatabase.FindAssets(
                "t:FeatureDef", new string[] { "Assets/Content/Features" });

            for (int i = 0; i < featureGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(featureGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                FeatureDef feat = AssetDatabase.LoadAssetAtPath<FeatureDef>(
                    assetPath);

                feat.SetID();

                string s = "";
                bool set = false;
                foreach (FieldInfo info in featConsts)
                {
                    if (feat.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(feat);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{feat.name} has no constant.");
            }
        }

        private static void PopulateDatabaseSpells(Pantheon.Core.Database db)
        {
            List<FieldInfo> spellConsts = GetConstants(typeof(ID.Spell));

            string[] spellGuids = AssetDatabase.FindAssets(
                "t:Spell", new string[] { "Assets/Content/Spells" });

            for (int i = 0; i < spellGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(spellGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Spell spell = AssetDatabase.LoadAssetAtPath<Spell>(
                    assetPath);

                spell.SetID();

                string s = "";
                bool set = false;
                foreach (FieldInfo info in spellConsts)
                {
                    if (spell.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(spell);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{spell.name} has no constant.");
            }
        }

        private static void PopulateDatabaseAspects(Pantheon.Core.Database db)
        {
            List<FieldInfo> aspectConsts = GetConstants(typeof(ID.Aspect));

            string[] aspectGuids = AssetDatabase.FindAssets(
                "t:Aspect", new string[] { "Assets/Content/Aspects" });

            for (int i = 0; i < aspectGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(aspectGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Aspect aspect = AssetDatabase.LoadAssetAtPath<Aspect>(
                    assetPath);

                aspect.SetID();

                string s = "";
                bool set = false;
                foreach (FieldInfo info in aspectConsts)
                {
                    if (aspect.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(aspect);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{aspect.name} has no constant.");
            }
        }

        private static void PopulateDatabaseSpecies(Pantheon.Core.Database db)
        {
            List<FieldInfo> speciesConsts = GetConstants(typeof(ID.Species));

            string[] speciesGuids = AssetDatabase.FindAssets(
                "t:Species", new string[] { "Assets/Content/Species" });

            for (int i = 0; i < speciesGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(speciesGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Species species = AssetDatabase.LoadAssetAtPath<Species>(
                    assetPath);

                species.SetID();

                string s = "";
                bool set = false;
                foreach (FieldInfo info in speciesConsts)
                {
                    if (species.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(species);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{species.name} has no constant.");
            }
        }

        private static void PopulateDatabaseOccupations(Pantheon.Core.Database db)
        {
            List<FieldInfo> occConsts = GetConstants(typeof(ID.Occupation));

            string[] occGuids = AssetDatabase.FindAssets(
                "t:Occupation", new string[] { "Assets/Content/Occupations" });

            for (int i = 0; i < occGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(occGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Occupation occ = AssetDatabase.LoadAssetAtPath<Occupation>(
                    assetPath);

                occ.SetID();

                string s = ""; // Dummy string for FieldInfo.GetValue()
                bool set = false;
                foreach (FieldInfo info in occConsts)
                {
                    if (occ.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(occ);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{occ.name} has no constant.");
            }
        }

        private static void PopulateDatabaseLandmarks(Pantheon.Core.Database db)
        {
            List<FieldInfo> landmarkConsts = GetConstants(typeof(ID.Landmark));

            string[] landmarkGuids = AssetDatabase.FindAssets(
                "t:Landmark", new string[] { "Assets/Prefabs/Landmarks" });

            for (int i = 0; i < landmarkGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(landmarkGuids[i]);
                Debug.Log($"Adding {assetPath}...");
                Landmark landmark = AssetDatabase.LoadAssetAtPath<Landmark>(
                    assetPath);

                landmark.SetID();

                string s = ""; // Dummy string for FieldInfo.GetValue()
                bool set = false;
                foreach (FieldInfo info in landmarkConsts)
                {
                    if (landmark.ID == (string)info.GetValue(s))
                    {
                        db.Content.Add(landmark);
                        set = true;
                    }
                }
                if (!set)
                    Debug.LogWarning($"{landmark.name} has no constant.");
            }
        }
    }
}

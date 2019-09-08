// Database.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using Pantheon.World;

namespace Pantheon.Core
{
    /// <summary>
    /// In-game database holding all template data.
    /// </summary>
    public sealed class Database : MonoBehaviour
    {
        // Database lists
        [SerializeField] private List<WeaponData> weaponList = new List<WeaponData>();
        [SerializeField] private List<Corpse> corpseList = new List<Corpse>();
        [SerializeField] private List<ScrollData> scrollList = new List<ScrollData>();
        [SerializeField] private List<FlaskData> flaskList = new List<FlaskData>();
        [SerializeField] private List<TerrainData> terrainList = new List<TerrainData>();
        [SerializeField] private List<NPCWrapper> NPCList = new List<NPCWrapper>();
        [SerializeField] private List<Feature> features = new List<Feature>();

        // Miscellaneous
        public Sprite lineTargetOverlay;
        public static Sprite LineTargetOverlay
            => GetDatabase().lineTargetOverlay;

        // Dictionaries for lookup by enum
        public Dictionary<WeaponType, WeaponData> WeaponDict { get; }
            = new Dictionary<WeaponType, WeaponData>();
        public Dictionary<CorpseType, Corpse> CorpseDict { get; }
            = new Dictionary<CorpseType, Corpse>();
        public Dictionary<ScrollType, ScrollData> ScrollDict { get; }
            = new Dictionary<ScrollType, ScrollData>();
        public Dictionary<FlaskType, FlaskData> FlaskDict { get; }
            = new Dictionary<FlaskType, FlaskData>();
        public Dictionary<TerrainType, TerrainData> TerrainDict { get; }
            = new Dictionary<TerrainType, TerrainData>();
        public Dictionary<NPCType, NPCWrapper> NPCDict { get; }
            = new Dictionary<NPCType, NPCWrapper>();
        public Dictionary<FeatureType, Feature> FeatureDict { get; }
            = new Dictionary<FeatureType, Feature>();

        // Awake is called when the script instance is being loaded
        private void Awake() => InitDatabaseDicts();

        private static Database GetDatabase() => Game.instance.Database;

        /// <summary>
        /// Initialize each of the database's dictionaries.
        /// </summary>
        private void InitDatabaseDicts()
        {
            for (int i = 0; i < weaponList.Count; i++)
                WeaponDict.Add(weaponList[i].Type, weaponList[i]);
            for (int i = 0; i < corpseList.Count; i++)
                CorpseDict.Add(corpseList[i].CorpseType, corpseList[i]);
            for (int i = 0; i < scrollList.Count; i++)
                ScrollDict.Add(scrollList[i].ScrollType, scrollList[i]);
            for (int i = 0; i < flaskList.Count; i++)
                FlaskDict.Add(flaskList[i].FlaskType, flaskList[i]);
            for (int i = 0; i < terrainList.Count; i++)
                TerrainDict.Add(terrainList[i]._terrainType, terrainList[i]);
            for (int i = 0; i < NPCList.Count; i++)
                NPCDict.Add(NPCList[i].Type, NPCList[i]);
            for (int i = 0; i < features.Count; i++)
                FeatureDict.Add(features[i].Type, features[i]);
        }

        #region Accessors

        // Get weapon data by enum
        public static WeaponData GetWeapon(WeaponType type)
        {
            GetDatabase().WeaponDict.TryGetValue(type, out WeaponData ret);

            if (ret == null)
                throw new Exception("Failed to get specified weapon data.");

            return ret;
        }

        // Get corpse data by enum
        public static Corpse GetCorpse(CorpseType corpseType)
        {
            GetDatabase().CorpseDict.TryGetValue(corpseType, out Corpse ret);

            if (ret == null)
                throw new Exception("Failed to get specified corpse data.");

            return ret;
        }

        // Get scroll data by enum
        public static ScrollData GetScroll(ScrollType scrollType)
        {
            GetDatabase().ScrollDict.TryGetValue(scrollType, out ScrollData ret);

            if (ret == null)
                throw new Exception("Failed to get specified scroll data.");

            return ret;
        }

        // Get potion data by enum
        public static FlaskData GetFlask(FlaskType flaskType)
        {
            GetDatabase().FlaskDict.TryGetValue(flaskType, out FlaskData ret);

            if (ret == null)
                throw new Exception("Failed to get specified flask data.");

            return ret;
        }

        // Get terrain data by enum
        public static TerrainData GetTerrain(TerrainType terrainType)
        {
            GetDatabase().TerrainDict.TryGetValue(terrainType, out TerrainData ret);

            if (ret == null)
                throw new Exception("Failed to get specified terrain data.");

            return ret;
        }

        /// <summary>
        /// Get an NPC prefab by NPCType.
        /// </summary>
        /// <param name="npcType">The enumerated type of the NPC.</param>
        /// <returns>The prefab corresponding to npcType.</returns>
        public static NPCWrapper GetNPC(NPCType npcType)
        {
            GetDatabase().NPCDict.TryGetValue(npcType, out NPCWrapper ret);

            if (ret == null)
                throw new Exception("Failed to get specified NPC.");

            return ret;
        }

        // Get feature data by enum
        public static Feature GetFeature(FeatureType featureType)
        {
            GetDatabase().FeatureDict.TryGetValue(featureType, out Feature ret);

            if (ret == null)
                throw new Exception("Failed to get specified feature.");

            return ret;
        }

        #endregion
    }
}

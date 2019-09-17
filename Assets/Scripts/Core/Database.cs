// Database.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.World;

namespace Pantheon.Core
{
    /// <summary>
    /// In-game database holding all template data.
    /// </summary>
    public sealed class Database : MonoBehaviour
    {
        private static Database GetDatabase() => Game.instance.Database;

        // Database lists
        [SerializeField] private List<WeaponData> weaponList = new List<WeaponData>();
        [SerializeField] private List<ScrollData> scrollList = new List<ScrollData>();
        [SerializeField] private List<FlaskData> flaskList = new List<FlaskData>();
        [SerializeField] private List<TerrainData> terrainList = new List<TerrainData>();
        [SerializeField] private List<NPCWrapper> NPCList = new List<NPCWrapper>();
        [SerializeField] private List<Feature> features = new List<Feature>();
        [SerializeField] private List<Spell> spells = new List<Spell>();
        [SerializeField] private List<AmmoData> ammoList = new List<AmmoData>();

        // Miscellaneous
        [SerializeField] private Tile unknownTerrain = null;
        public static Tile UnknownTerrain
            => GetDatabase().unknownTerrain;
        [SerializeField] private Sprite lineTargetOverlay = null;
        public static Sprite LineTargetOverlay
            => GetDatabase().lineTargetOverlay;
        [SerializeField] private GameObject tossFXPrefab = null;
        public static GameObject TossFXPrefab
            => GetDatabase().tossFXPrefab;

        // Dictionaries for lookup by enum
        public Dictionary<WeaponType, WeaponData> WeaponDict { get; }
            = new Dictionary<WeaponType, WeaponData>();
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
        public Dictionary<SpellType, Spell> SpellDict { get; }
            = new Dictionary<SpellType, Spell>();
        public Dictionary<AmmoType, AmmoData> AmmoDict { get; }
            = new Dictionary<AmmoType, AmmoData>();

        // Awake is called when the script instance is being loaded
        private void Awake() => InitDatabaseDicts();

        /// <summary>
        /// Initialize each of the database's dictionaries.
        /// </summary>
        private void InitDatabaseDicts()
        {
            for (int i = 0; i < weaponList.Count; i++)
                WeaponDict.Add(weaponList[i].Type, weaponList[i]);
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
            for (int i = 0; i < spells.Count; i++)
                SpellDict.Add(spells[i].Type, spells[i]);
            for (int i = 0; i < ammoList.Count; i++)
                AmmoDict.Add(ammoList[i].AmmoType, ammoList[i]);
        }

        #region Accessors

        public static WeaponData GetWeapon(WeaponType type)
        {
            GetDatabase().WeaponDict.TryGetValue(type, out WeaponData ret);

            if (ret == null)
                throw new Exception("Failed to get specified weapon data.");

            return ret;
        }

        public static ScrollData GetScroll(ScrollType scrollType)
        {
            GetDatabase().ScrollDict.TryGetValue(scrollType, out ScrollData ret);

            if (ret == null)
                throw new Exception("Failed to get specified scroll data.");

            return ret;
        }

        public static FlaskData GetFlask(FlaskType flaskType)
        {
            GetDatabase().FlaskDict.TryGetValue(flaskType, out FlaskData ret);

            if (ret == null)
                throw new Exception("Failed to get specified flask data.");

            return ret;
        }

        public static TerrainData GetTerrain(TerrainType terrainType)
        {
            GetDatabase().TerrainDict.TryGetValue(terrainType, out TerrainData ret);

            if (ret == null)
                throw new Exception("Failed to get specified terrain data.");

            return ret;
        }

        public static NPCWrapper GetNPC(NPCType npcType)
        {
            GetDatabase().NPCDict.TryGetValue(npcType, out NPCWrapper ret);

            if (ret == null)
                throw new Exception("Failed to get specified NPC.");

            return ret;
        }

        public static Feature GetFeature(FeatureType featureType)
        {
            GetDatabase().FeatureDict.TryGetValue(featureType, out Feature ret);

            if (ret == null)
                throw new Exception("Failed to get specified feature.");

            return ret;
        }

        public static Spell GetSpell(SpellType spellType)
        {
            GetDatabase().SpellDict.TryGetValue(spellType, out Spell ret);

            if (ret == null)
                throw new Exception("Failed to get specified spell.");

            return ret;
        }

        public static AmmoData GetAmmo(AmmoType ammoType)
        {
            GetDatabase().AmmoDict.TryGetValue(ammoType, out AmmoData ret);

            if (ret == null)
                throw new Exception("Failed to get specified ammo.");

            return ret;
        }

        #endregion
    }
}

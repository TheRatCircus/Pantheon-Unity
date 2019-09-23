// Database.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;

namespace Pantheon.Core
{
    /// <summary>
    /// In-game database holding all template data.
    /// </summary>
    public sealed class Database : MonoBehaviour
    {
        private static Database GetDatabase() => Game.instance.Database;

        // Database lists
        [SerializeField] private List<WeaponData> weaponList
            = new List<WeaponData>();
        [SerializeField] private List<ScrollData> scrollList
            = new List<ScrollData>();
        [SerializeField] private List<FlaskData> flaskList
            = new List<FlaskData>();
        [SerializeField] private List<TerrainData> terrainList
            = new List<TerrainData>();
        [SerializeField] private List<NPCWrapper> NPCList
            = new List<NPCWrapper>();
        [SerializeField] private List<FeatureData> features
            = new List<FeatureData>();
        [SerializeField] private List<Spell> spells
            = new List<Spell>();
        [SerializeField] private List<AmmoData> ammoList
            = new List<AmmoData>();
        [SerializeField] private List<Aspect> aspects
            = new List<Aspect>();
        [SerializeField] private List<Species> species
            = new List<Species>();
        [SerializeField] private List<Occupation> occupations
            = new List<Occupation>();
        [SerializeField] private List<ArmourData> armours
            = new List<ArmourData>();

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
        public Dictionary<FeatureType, FeatureData> FeatureDict { get; }
            = new Dictionary<FeatureType, FeatureData>();
        public Dictionary<SpellType, Spell> SpellDict { get; }
            = new Dictionary<SpellType, Spell>();
        public Dictionary<AmmoType, AmmoData> AmmoDict { get; }
            = new Dictionary<AmmoType, AmmoData>();
        public Dictionary<SpeciesRef, Species> SpeciesDict { get; }
            = new Dictionary<SpeciesRef, Species>();
        public Dictionary<OccupationRef, Occupation> OccupationDict { get; }
            = new Dictionary<OccupationRef, Occupation>();
        public Dictionary<ArmourRef, ArmourData> ArmourDict { get; }
            = new Dictionary<ArmourRef, ArmourData>();

        // Miscellaneous
        [SerializeField] private GameObject genericNPC = null;
        public static GameObject GenericNPC => GetDatabase().genericNPC;

        [SerializeField] private Tile unknownTerrain = null;
        public static Tile UnknownTerrain
            => GetDatabase().unknownTerrain;

        [SerializeField] private Sprite lineTargetOverlay = null;
        public static Sprite LineTargetOverlay
            => GetDatabase().lineTargetOverlay;

        [SerializeField] private GameObject tossFXPrefab = null;
        public static GameObject TossFXPrefab
            => GetDatabase().tossFXPrefab;

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
                TerrainDict.Add(terrainList[i].TerrainType, terrainList[i]);
            for (int i = 0; i < NPCList.Count; i++)
                NPCDict.Add(NPCList[i].Type, NPCList[i]);
            for (int i = 0; i < features.Count; i++)
                FeatureDict.Add(features[i].Type, features[i]);
            for (int i = 0; i < spells.Count; i++)
                SpellDict.Add(spells[i].Type, spells[i]);
            for (int i = 0; i < ammoList.Count; i++)
                AmmoDict.Add(ammoList[i].AmmoType, ammoList[i]);
            for (int i = 0; i < species.Count; i++)
                SpeciesDict.Add(species[i].Reference, species[i]);
            for (int i = 0; i < occupations.Count; i++)
                OccupationDict.Add(occupations[i].Reference, occupations[i]);
            for (int i = 0; i < armours.Count; i++)
                ArmourDict.Add(armours[i].ArmourRef, armours[i]);
        }

        #region Accessors

        public static WeaponData GetWeapon(WeaponType type)
        {
            if (!GetDatabase().WeaponDict.TryGetValue(type, out WeaponData ret))
                throw new ArgumentException("Failed to get specified weapon data.");

            return ret;
        }

        public static ScrollData GetScroll(ScrollType scrollType)
        {
            if (!GetDatabase().ScrollDict.TryGetValue(scrollType, out ScrollData ret))
                throw new ArgumentException("Failed to get specified scroll data.");

            return ret;
        }

        public static FlaskData GetFlask(FlaskType flaskType)
        {
            if (!GetDatabase().FlaskDict.TryGetValue(flaskType, out FlaskData ret))
                throw new ArgumentException("Failed to get specified flask data.");

            return ret;
        }

        public static TerrainData GetTerrain(TerrainType terrainType)
        {
            if (!GetDatabase().TerrainDict.TryGetValue(terrainType, out TerrainData ret))
                throw new ArgumentException("Failed to get specified terrain data.");

            return ret;
        }

        public static NPCWrapper GetNPC(NPCType npcType)
        {
            if (!GetDatabase().NPCDict.TryGetValue(npcType,
                out NPCWrapper ret))
                throw new ArgumentException("Failed to get specified NPC.");

            return ret;
        }

        public static FeatureData GetFeature(FeatureType featureType)
        {
            if (!GetDatabase().FeatureDict.TryGetValue(featureType,
                out FeatureData ret))
                throw new ArgumentException
                    ("Failed to get specified feature.");

            return ret;
        }

        public static Spell GetSpell(SpellType spellType)
        {
            if (!GetDatabase().SpellDict.TryGetValue(spellType, out Spell ret))
                throw new ArgumentException("Failed to get specified spell.");

            return ret;
        }

        public static AmmoData GetAmmo(AmmoType ammoType)
        {
            if (!GetDatabase().AmmoDict.TryGetValue(ammoType,
                out AmmoData ret))
                throw new ArgumentException("Failed to get specified ammo.");

            return ret;
        }

        public static Occupation GetOccupation(OccupationRef occRef)
        {
            if (!GetDatabase().OccupationDict.TryGetValue(occRef,
                out Occupation ret))
                throw new ArgumentException("Failed to get specified occupation.");

            return ret;
        }

        public static Species GetSpecies(SpeciesRef speciesRef)
        {
            if (!GetDatabase().SpeciesDict.TryGetValue(speciesRef,
                out Species ret))
                throw new ArgumentException("Failed to get specified species.");

            return ret;
        }

        public static ArmourData GetArmour(ArmourRef armourRef)
        {
            if (!GetDatabase().ArmourDict.TryGetValue(armourRef,
                out ArmourData ret))
                throw new ArgumentException("Failed to get specified armour.");

            return ret;
        }

        public static Aspect RandomAspect()
        {
            Database db = GetDatabase();
            return db.aspects.Random(true);
        }

        #endregion
    }
}

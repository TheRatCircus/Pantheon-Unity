// Database.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Utils;
using Pantheon.WorldGen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.Core
{
    /// <summary>
    /// In-game database holding all template data.
    /// </summary>
    public sealed class Database : MonoBehaviour
    {
        private static Database GetDatabase() => Game.instance.Database;

        // Database lists
        [SerializeField]
        private List<ItemData> itemList =
            new List<ItemData>();
        public List<ItemData> ItemList => itemList;

        [SerializeField] private List<TerrainData> terrainList
            = new List<TerrainData>();
        [SerializeField] private List<NPCWrapper> NPCList
            = new List<NPCWrapper>();
        [SerializeField] private List<FeatureData> features
            = new List<FeatureData>();
        [SerializeField] private List<Spell> spells
            = new List<Spell>();
        [SerializeField] private List<Aspect> aspects
            = new List<Aspect>();
        [SerializeField] private List<Species> species
            = new List<Species>();
        [SerializeField] private List<Occupation> occupations
            = new List<Occupation>();
        [SerializeField] private List<Landmark> landmarkList
            = new List<Landmark>();

        // Dictionaries for lookup by enum
        public Dictionary<string, ItemData> ItemDict { get; private set; }
        public Dictionary<TerrainType, TerrainData> TerrainDict { get; }
            = new Dictionary<TerrainType, TerrainData>();
        public Dictionary<NPCType, NPCWrapper> NPCDict { get; }
            = new Dictionary<NPCType, NPCWrapper>();
        public Dictionary<FeatureType, FeatureData> FeatureDict { get; }
            = new Dictionary<FeatureType, FeatureData>();
        public Dictionary<SpellType, Spell> SpellDict { get; }
            = new Dictionary<SpellType, Spell>();
        public Dictionary<SpeciesRef, Species> SpeciesDict { get; }
            = new Dictionary<SpeciesRef, Species>();
        public Dictionary<OccupationRef, Occupation> OccupationDict { get; }
            = new Dictionary<OccupationRef, Occupation>();
        public Dictionary<LandmarkRef, Landmark> LandmarkDict { get; }
            = new Dictionary<LandmarkRef, Landmark>();

        // Miscellaneous
        public static List<Aspect> AspectList { get => GetDatabase().aspects; }

        [SerializeField] private GameObject genericNPC = null;
        public static GameObject GenericNPC => GetDatabase().genericNPC;

        [SerializeField] private Tile unknownTerrain = null;
        public static Tile UnknownTerrain
            => GetDatabase().unknownTerrain;

        [SerializeField] private Sprite lineTargetOverlay = null;
        public static Sprite LineTargetOverlay
            => GetDatabase().lineTargetOverlay;

        [SerializeField] private GameObject tossFXPrefab = null;
        public static GameObject TossFXPrefab => GetDatabase().tossFXPrefab;

        [SerializeField] private RuleTile splatterTile = null;
        public static RuleTile SplatterTile => GetDatabase().splatterTile;

        [SerializeField] private TextAsset relicNames = null;
        public static TextAsset RelicNames => GetDatabase().relicNames;

        // Awake is called when the script instance is being loaded
        private void Awake() => InitDatabaseDicts();

        /// <summary>
        /// Initialize each of the database's dictionaries.
        /// </summary>
        private void InitDatabaseDicts()
        {
            ItemDict = new Dictionary<string, ItemData>(itemList.Count);

            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ID == "NO_REF")
                    throw new Exception($"{itemList[i].name} has no ID.");
                ItemDict.Add(itemList[i].ID, itemList[i]);
            }
            for (int i = 0; i < terrainList.Count; i++)
                TerrainDict.Add(terrainList[i].TerrainType, terrainList[i]);
            for (int i = 0; i < NPCList.Count; i++)
                NPCDict.Add(NPCList[i].Type, NPCList[i]);
            for (int i = 0; i < features.Count; i++)
                FeatureDict.Add(features[i].Type, features[i]);
            for (int i = 0; i < spells.Count; i++)
                SpellDict.Add(spells[i].Type, spells[i]);
            for (int i = 0; i < species.Count; i++)
                SpeciesDict.Add(species[i].Reference, species[i]);
            for (int i = 0; i < occupations.Count; i++)
                OccupationDict.Add(occupations[i].Reference, occupations[i]);
            for (int i = 0; i < landmarkList.Count; i++)
                LandmarkDict.Add(landmarkList[i].Reference, landmarkList[i]);
        }

        #region Accessors

        public static ItemData GetItem(string itemID)
        {
            if (!GetDatabase().ItemDict.TryGetValue(itemID, out ItemData ret))
                throw new ArgumentException(
                    $"Failed to get item: {itemID}");

            return ret;
        }

        public static TerrainData GetTerrain(TerrainType terrainType)
        {
            if (!GetDatabase().TerrainDict.TryGetValue(terrainType,
                out TerrainData ret))
                throw new ArgumentException($"Failed to get terrain data: " +
                    $"{terrainType}.");

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
                throw new ArgumentException
                    ($"Failed to get species {speciesRef}.");

            return ret;
        }

        public static Landmark GetLandmark(LandmarkRef landmarkRef)
        {
            if (!GetDatabase().LandmarkDict.TryGetValue(landmarkRef,
                out Landmark ret))
                throw new ArgumentException($"Failed to get landmark data" +
                    $" {landmarkRef}.");

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

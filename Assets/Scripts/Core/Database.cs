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

        [SerializeField]
        private List<ItemDef> itemList =
            new List<ItemDef>();
        [SerializeField]
        private List<TerrainDef> terrainList =
            new List<TerrainDef>();
        [SerializeField]
        private List<NPCWrapper> npcList =
            new List<NPCWrapper>();
        [SerializeField]
        private List<FeatureDef> featureList =
            new List<FeatureDef>();
        [SerializeField]
        private List<Spell> spellList =
            new List<Spell>();
        [SerializeField]
        private List<Aspect> aspectList =
            new List<Aspect>();
        [SerializeField]
        private List<Species> speciesList =
            new List<Species>();
        [SerializeField]
        private List<Occupation> occupationList =
            new List<Occupation>();
        [SerializeField]
        private List<Landmark> landmarkList =
            new List<Landmark>();

        public List<ItemDef> ItemList => itemList;
        public List<TerrainDef> TerrainList => terrainList;
        public List<NPCWrapper> NPCList => npcList;
        public List<FeatureDef> FeatureList => featureList;
        public List<Spell> SpellList => spellList;
        public List<Aspect> AspectList => aspectList;
        public List<Species> SpeciesList => speciesList;
        public List<Occupation> OccupationList => occupationList; 
        public List<Landmark> LandmarkList => landmarkList;

        // Dictionaries for lookup by enum
        public Dictionary<ItemID, ItemDef> ItemDict
        { get; private set; } = new Dictionary<ItemID, ItemDef>();
        public Dictionary<TerrainID, TerrainDef> TerrainDict
        { get; private set; } = new Dictionary<TerrainID, TerrainDef>();
        public Dictionary<NPCID, NPCWrapper> NPCDict
        { get; private set; } = new Dictionary<NPCID, NPCWrapper>();
        public Dictionary<FeatureID, FeatureDef> FeatureDict
        { get; private set; } = new Dictionary<FeatureID, FeatureDef>();
        public Dictionary<SpellID, Spell> SpellDict
        { get; private set; } = new Dictionary<SpellID, Spell>();
        public Dictionary<SpeciesID, Species> SpeciesDict
        { get; private set; } = new Dictionary<SpeciesID, Species>();
        public Dictionary<OccupationID, Occupation> OccupationDict
        { get; private set; } = new Dictionary<OccupationID, Occupation>();
        public Dictionary<LandmarkID, Landmark> LandmarkDict
        { get; private set; } = new Dictionary<LandmarkID, Landmark>();
        public Dictionary<AspectID, Aspect> AspectDict
        { get; private set; } = new Dictionary<AspectID, Aspect>();

        [SerializeField] private GameObject genericNPC = null;
        public static GameObject GenericNPC => GetDatabase().genericNPC;

        [SerializeField] private Sprite lineTargetOverlay = null;
        public static Sprite LineTargetOverlay
            => GetDatabase().lineTargetOverlay;

        [SerializeField] private GameObject tossFXPrefab = null;
        public static GameObject TossFXPrefab => GetDatabase().tossFXPrefab;

        [SerializeField] private RuleTile splatterTile = null;
        public static RuleTile SplatterTile => GetDatabase().splatterTile;

        [SerializeField] private TextAsset relicNames = null;
        public static TextAsset RelicNames => GetDatabase().relicNames;

        private void Awake()
        {
            ItemDict = new Dictionary<ItemID, ItemDef>(ItemList.Count);
            for (int i = 0; i < ItemList.Count; i++)
                ItemDict.Add(ItemList[i].ID, ItemList[i]);

            TerrainDict = new Dictionary<TerrainID, TerrainDef>(TerrainList.Count);
            for (int i = 0; i < TerrainList.Count; i++)
                TerrainDict.Add(TerrainList[i].ID, TerrainList[i]);

            NPCDict = new Dictionary<NPCID, NPCWrapper>(NPCList.Count);
            for (int i = 0; i < NPCList.Count; i++)
                NPCDict.Add(NPCList[i].ID, NPCList[i]);

            FeatureDict = new Dictionary<FeatureID, FeatureDef>(FeatureList.Count);
            for (int i = 0; i < FeatureList.Count; i++)
                FeatureDict.Add(FeatureList[i].ID, FeatureList[i]);

            SpellDict = new Dictionary<SpellID, Spell>(SpellList.Count);
            for (int i = 0; i < SpellList.Count; i++)
                SpellDict.Add(SpellList[i].ID, SpellList[i]);

            AspectDict = new Dictionary<AspectID, Aspect>(AspectList.Count);
            for (int i = 0; i < AspectList.Count; i++)
                AspectDict.Add(AspectList[i].ID, AspectList[i]);

            SpeciesDict = new Dictionary<SpeciesID, Species>(SpeciesList.Count);
            for (int i = 0; i < SpeciesList.Count; i++)
                SpeciesDict.Add(SpeciesList[i].ID, SpeciesList[i]);

            OccupationDict = new Dictionary<OccupationID, Occupation>(OccupationList.Count);
            for (int i = 0; i < OccupationList.Count; i++)
                OccupationDict.Add(OccupationList[i].ID, OccupationList[i]);

            LandmarkDict = new Dictionary<LandmarkID, Landmark>(LandmarkList.Count);
            for (int i = 0; i < LandmarkList.Count; i++)
                LandmarkDict.Add(LandmarkList[i].ID, LandmarkList[i]);
        }

        #region Accessors

        public static ItemDef GetItem(ItemID id)
        {
            if (!GetDatabase().ItemDict.TryGetValue(id, out ItemDef ret))
                throw new ArgumentException(
                    $"Failed to get item: {id}");

            return ret;
        }

        public static TerrainDef GetTerrain(TerrainID id)
        {
            if (!GetDatabase().TerrainDict.TryGetValue(id,
                out TerrainDef ret))
                throw new ArgumentException(
                    $"Failed to get terrain: {id}");

            return ret;
        }

        public static NPCWrapper GetNPC(NPCID id)
        {
            if (!GetDatabase().NPCDict.TryGetValue(id,
                out NPCWrapper ret))
                throw new ArgumentException($"Failed to get NPC: {id}");

            return ret;
        }

        public static FeatureDef GetFeature(FeatureID id)
        {
            if (!GetDatabase().FeatureDict.TryGetValue(id,
                out FeatureDef ret))
                throw new ArgumentException
                    ($"Failed to get feature: {id}");

            return ret;
        }

        public static Spell GetSpell(SpellID id)
        {
            if (!GetDatabase().SpellDict.TryGetValue(id, out Spell ret))
                throw new ArgumentException($"Failed to get spell: {id}");

            return ret;
        }

        public static Occupation GetOccupation(OccupationID id)
        {
            if (!GetDatabase().OccupationDict.TryGetValue(id,
                out Occupation ret))
                throw new ArgumentException($"Failed to get occupation: {id}");

            return ret;
        }

        public static Species GetSpecies(SpeciesID id)
        {
            if (!GetDatabase().SpeciesDict.TryGetValue(id,
                out Species ret))
                throw new ArgumentException(
                    $"Failed to get species {id}");

            return ret;
        }

        public static Landmark GetLandmark(LandmarkID id)
        {
            if (!GetDatabase().LandmarkDict.TryGetValue(id,
                out Landmark ret))
                throw new ArgumentException(
                    $"Failed to get landmark: {id}");

            return ret;
        }

        public static Aspect RandomAspect()
        {
            return GetDatabase().AspectDict.Random(true);
        }

        #endregion
    }
}

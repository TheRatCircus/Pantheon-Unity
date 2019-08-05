// In-game database holding all base data
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    // Singleton
    public static Database instance;

    // Database lists
    public List<Corpse> Corpses = new List<Corpse>();
    public List<Potion> Potions = new List<Potion>();
    public List<TerrainData> Terrain = new List<TerrainData>();

    // Dictionaries for lookup by enum
    public Dictionary<CorpseType, Corpse> CorpseDict = new Dictionary<CorpseType, Corpse>();
    public Dictionary<PotionType, Potion> PotionDict = new Dictionary<PotionType, Potion>();
    public Dictionary<TerrainType, TerrainData> TerrainDict = new Dictionary<TerrainType, TerrainData>();
    
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Database singleton assigned in error");
        else
            instance = this;

        InitCorpseDict();
        InitPotionDict();
        InitTerrainDict();
    }

    #region Init

    // Initialize the corpse dictionary
    private void InitCorpseDict()
    {
        for (int i = 0; i < Corpses.Count; i++)
            CorpseDict.Add((Corpses[i])._corpseType, Corpses[i]);
    }

    // Initialize the potion dictionary
    private void InitPotionDict()
    {
        for (int i = 0; i < Potions.Count; i++)
            PotionDict.Add((Potions[i])._potionType, Potions[i]);
    }

    // Initialize the terrain tile dictionary
    private void InitTerrainDict()
    {
        for (int i = 0; i < Terrain.Count; i++)
            TerrainDict.Add(Terrain[i]._terrainType, Terrain[i]);
    }

    #endregion

    #region StaticAccessors

    // Get corpse data by enum
    public static Corpse GetCorpse(CorpseType corpseType)
    {
        instance.CorpseDict.TryGetValue(corpseType, out Corpse ret);
        return ret;
    }

    // Get potion data by enum
    public static Potion GetPotion(PotionType potionType)
    {
        instance.PotionDict.TryGetValue(potionType, out Potion ret);
        return ret;
    }

    // Get a terrain data by enum
    public static TerrainData GetTerrain(TerrainType terrainType)
    {
        instance.TerrainDict.TryGetValue(terrainType, out TerrainData ret);
        return ret;
    }

    #endregion
}

// In-game database holding all base data
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    // Singleton
    public static Database instance;

    // Database lists
    public List<Corpse> Corpses = new List<Corpse>();
    public List<Scroll> Scrolls = new List<Scroll>();
    public List<Flask> Flasks = new List<Flask>();
    public List<TerrainData> Terrain = new List<TerrainData>();

    // Dictionaries for lookup by enum
    public Dictionary<CorpseType, Corpse> CorpseDict = new Dictionary<CorpseType, Corpse>();
    public Dictionary<ScrollType, Scroll> ScrollDict = new Dictionary<ScrollType, Scroll>();
    public Dictionary<FlaskType, Flask> FlaskDict = new Dictionary<FlaskType, Flask>();
    public Dictionary<TerrainType, TerrainData> TerrainDict = new Dictionary<TerrainType, TerrainData>();

    // Miscellaneous
    public Sprite lineTargetOverlay;
    
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Database singleton assigned in error");
        else
            instance = this;

        InitCorpseDict();
        InitScrollDict();
        InitFlaskDict();
        InitTerrainDict();
    }

    #region Init

    // Initialize the corpse dictionary
    private void InitCorpseDict()
    {
        for (int i = 0; i < Corpses.Count; i++)
            CorpseDict.Add((Corpses[i])._corpseType, Corpses[i]);
    }

    // Initialize the scroll dictionary
    private void InitScrollDict()
    {
        for (int i = 0; i < Scrolls.Count; i++)
            ScrollDict.Add((Scrolls[i])._scrollType, Scrolls[i]);
    }

    // Initialize the potion dictionary
    private void InitFlaskDict()
    {
        for (int i = 0; i < Flasks.Count; i++)
            FlaskDict.Add((Flasks[i])._flaskType, Flasks[i]);
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
    
    // Get scroll data by enum
    public static Scroll GetScroll(ScrollType scrollType)
    {
        instance.ScrollDict.TryGetValue(scrollType, out Scroll ret);
        return ret;
    }

    // Get potion data by enum
    public static Flask GetFlask(FlaskType flaskType)
    {
        instance.FlaskDict.TryGetValue(flaskType, out Flask ret);
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

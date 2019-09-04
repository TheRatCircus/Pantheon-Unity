// In-game database holding all base data
using System.Collections.Generic;
using UnityEngine;

public sealed class Database : MonoBehaviour
{
    // Database lists
    public List<Corpse> Corpses = new List<Corpse>();
    public List<ScrollData> Scrolls = new List<ScrollData>();
    public List<FlaskData> Flasks = new List<FlaskData>();
    public List<TerrainData> Terrain = new List<TerrainData>();
    public List<NPCWrapper> NPC = new List<NPCWrapper>();

    // Dictionaries for lookup by enum
    public Dictionary<CorpseType, Corpse> CorpseDict = new Dictionary<CorpseType, Corpse>();
    public Dictionary<ScrollType, ScrollData> ScrollDict = new Dictionary<ScrollType, ScrollData>();
    public Dictionary<FlaskType, FlaskData> FlaskDict = new Dictionary<FlaskType, FlaskData>();
    public Dictionary<TerrainType, TerrainData> TerrainDict = new Dictionary<TerrainType, TerrainData>();
    public Dictionary<NPCType, NPCWrapper> NPCDict = new Dictionary<NPCType, NPCWrapper>();

    // Miscellaneous
    public Sprite lineTargetOverlay;
    public static Sprite LineTargetOverlay
        => GetDatabase().lineTargetOverlay;

    // Awake is called when the script instance is being loaded
    private void Awake()
        => InitDatabaseDicts();

    private static Database GetDatabase() => Game.instance.database;

    /// <summary>
    /// Initialize each of the database's dictionaries.
    /// </summary>
    private void InitDatabaseDicts()
    {
        for (int i = 0; i < Corpses.Count; i++)
            CorpseDict.Add((Corpses[i])._corpseType, Corpses[i]);
        for (int i = 0; i < Scrolls.Count; i++)
            ScrollDict.Add((Scrolls[i])._scrollType, Scrolls[i]);
        for (int i = 0; i < Flasks.Count; i++)
            FlaskDict.Add(Flasks[i]._flaskType, Flasks[i]);
        for (int i = 0; i < Terrain.Count; i++)
            TerrainDict.Add(Terrain[i]._terrainType, Terrain[i]);
        for (int i = 0; i < NPC.Count; i++)
            NPCDict.Add(NPC[i].Type, NPC[i]);
    }

    #region StaticAccessors

    // Get corpse data by enum
    public static Corpse GetCorpse(CorpseType corpseType)
    {
        GetDatabase().CorpseDict.TryGetValue(corpseType, out Corpse ret);
        return ret;
    }
    
    // Get scroll data by enum
    public static ScrollData GetScroll(ScrollType scrollType)
    {
        GetDatabase().ScrollDict.TryGetValue(scrollType, out ScrollData ret);
        return ret;
    }

    // Get potion data by enum
    public static FlaskData GetFlask(FlaskType flaskType)
    {
        GetDatabase().FlaskDict.TryGetValue(flaskType, out FlaskData ret);
        return ret;
    }

    // Get terrain data by enum
    public static TerrainData GetTerrain(TerrainType terrainType)
    {
        GetDatabase().TerrainDict.TryGetValue(terrainType, out TerrainData ret);
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
        return ret;
    }

    #endregion
}

// Template for terrain type data
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Terrain", menuName = "Terrain")]
public class TerrainData : ScriptableObject
{
    public string DisplayName;
    public string RefName;
    public TerrainType _terrainType;

    public bool Opaque;
    public bool Blocked;

    public Tile _tile;
}
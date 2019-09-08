// TerrainData.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Attributes of a terrain type.
/// </summary>
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
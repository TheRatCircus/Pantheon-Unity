// TerrainData.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Attributes of a terrain type.
    /// </summary>
    [CreateAssetMenu(fileName = "New Terrain", menuName = "BaseData/Terrain")]
    public class TerrainData : ScriptableObject
    {
        [SerializeField] private string displayName = "TERRAIN_DISPLAY";
        [SerializeField] private string refName = "TERRAIN_REF";
        [SerializeField] private TerrainType terrainType = TerrainType.None;

        [SerializeField] private bool opaque = false;
        [SerializeField] private bool blocked = false;

        [SerializeField] private RuleTile ruleTile = null;

        #region Properties

        public string DisplayName { get => displayName; }
        public string RefName { get => refName; }
        public TerrainType TerrainType { get => terrainType; }

        public bool Opaque { get => opaque; private set => opaque = value; }
        public bool Blocked { get => blocked; private set => blocked = value; }

        public RuleTile RuleTile { get => ruleTile; }

        #endregion
    }

    public enum TerrainType
    {
        None,
        StoneWall,
        StoneFloor,
        Grass,
        MarbleTile
    }
}

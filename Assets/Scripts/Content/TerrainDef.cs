// TerrainDef.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Attributes of a terrain type.
    /// </summary>
    [CreateAssetMenu(fileName = "New Terrain",
        menuName = "Pantheon/Content/Terrain")]
    public sealed class TerrainDef : ScriptableObject
    {
        [SerializeField] private string displayName = "TERRAIN_DISPLAY";
        [SerializeField] private TerrainID id = TerrainID.Default;

        [SerializeField] private bool opaque = false;
        [SerializeField] private bool blocked = false;

        [SerializeField] private RuleTile ruleTile = null;

        #region Properties

        public string DisplayName { get => displayName; }
        public TerrainID ID { get => id; }

        public bool Opaque { get => opaque; private set => opaque = value; }
        public bool Blocked { get => blocked; private set => blocked = value; }

        public RuleTile RuleTile { get => ruleTile; }

        #endregion
    }

    public enum TerrainID
    {
        Default,
        StoneWall,
        StoneFloor,
        Grass,
        MarbleTile
    }
}

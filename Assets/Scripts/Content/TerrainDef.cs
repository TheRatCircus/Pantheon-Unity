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
    public sealed class TerrainDef : Content
    {
        [SerializeField] private string displayName = "TERRAIN_DISPLAY";

        [SerializeField] private bool opaque = false;
        [SerializeField] private bool blocked = false;

        [SerializeField] private RuleTile ruleTile = null;

        #region Properties

        public string DisplayName { get => displayName; }

        public bool Opaque { get => opaque; private set => opaque = value; }
        public bool Blocked { get => blocked; private set => blocked = value; }

        public RuleTile RuleTile { get => ruleTile; }

        #endregion
    }
}

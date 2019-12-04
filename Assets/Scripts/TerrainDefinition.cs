// TerrainDefinition.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary> Flyweight for cell terrains. </summary>
    [CreateAssetMenu(fileName = "New Terrain", menuName = "Pantheon/Content/Terrain")]
    public sealed class TerrainDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "DEFAULT_TERRAIN_NAME";
        [SerializeField] private RuleTile tile = default;
        [SerializeField] private Sprite sprite = default;
        [SerializeField] private bool blocked = false;
        [SerializeField] private bool opaque = false;

        public string DisplayName => displayName;
        public RuleTile Tile => tile;
        public Sprite Sprite => sprite;
        public bool Blocked => blocked;
        public bool Opaque => opaque;
    }
}

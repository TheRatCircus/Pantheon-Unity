// FeatureDef.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// The attributes of a cell feature, e.g. a tree or door.
    /// </summary>
    [CreateAssetMenu(fileName = "New Feature",
        menuName = "Pantheon/Content/Feature")]
    public class FeatureDef : ScriptableObject
    {
        [SerializeField] private string displayName = "DEFAULT_FEAT_NAME";
        [SerializeField] private bool opaque = false;
        [SerializeField] private bool blocked = false;
        [SerializeField] private Sprite sprite = null;
        [SerializeField] private RuleTile ruleTile = null;
        [SerializeField] private FeatureID id = FeatureID.Default;

        public string DisplayName { get => displayName; }
        public bool Opaque { get => opaque;}
        public bool Blocked { get => blocked; }
        public Sprite Sprite { get => sprite; }
        public RuleTile RuleTile { get => ruleTile; }
        public FeatureID ID { get => id; }
    }

    public enum FeatureID
    {
        Default,
        StairsUp,
        StairsDown,
        Tree,
        TrailNorth,
        TrailEast,
        TrailSouth,
        TrailWest,
        Portal,
        AltarCrystal,
        WoodFence
    }
}

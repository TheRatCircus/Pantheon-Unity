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
    public sealed class FeatureDef : Content
    {
        [SerializeField] private string displayName = "DEFAULT_FEAT_NAME";
        [SerializeField] private bool opaque = false;
        [SerializeField] private bool blocked = false;
        [SerializeField] private Sprite sprite = null;
        [SerializeField] private RuleTile ruleTile = null;

        public string DisplayName { get => displayName; }
        public bool Opaque { get => opaque;}
        public bool Blocked { get => blocked; }
        public Sprite Sprite { get => sprite; }
        public RuleTile RuleTile { get => ruleTile; }
    }
}

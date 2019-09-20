// FeatureData.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// The attributes of a cell feature, e.g. a tree or door.
    /// </summary>
    [CreateAssetMenu(fileName = "New Feature", menuName = "Feature")]
    public class FeatureData : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private bool opaque;
        [SerializeField] private bool blocked;
        [SerializeField] private Sprite sprite;
        [SerializeField] private FeatureType type;

        public string DisplayName { get => displayName; private set => displayName = value; }
        public bool Opaque { get => opaque; private set => opaque = value; }
        public bool Blocked { get => blocked; private set => blocked = value; }
        public Sprite Sprite { get => sprite; private set => sprite = value; }
        public FeatureType Type { get => type; private set => type = value; }
    }
}

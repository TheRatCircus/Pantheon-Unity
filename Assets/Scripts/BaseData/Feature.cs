// Feature.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.World
{
    [CreateAssetMenu(fileName = "New Feature", menuName = "Feature")]
    public class Feature : ScriptableObject
    {
        [SerializeField] private bool opaque;
        [SerializeField] private bool blocked;
        [SerializeField] private Sprite sprite;
        [SerializeField] private FeatureType type;

        public bool Opaque { get => opaque; private set => opaque = value; }
        public bool Blocked { get => blocked; private set => blocked = value; }
        public Sprite Sprite { get => sprite; private set => sprite = value; }
        public FeatureType Type { get => type; private set => type = value; }
    }
}

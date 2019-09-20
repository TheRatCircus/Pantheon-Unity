// Aspect.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.World;

namespace Pantheon.Core
{
    [CreateAssetMenu(fileName = "New Aspect", menuName = "Aspect")]
    public class Aspect : ScriptableObject
    {
        [SerializeField] private string displayName = "NO_ASPECT_NAME";
        [SerializeField] private string refName = "NO_ASPECT_REF";
        [SerializeField] private Sprite icon = null;

        [SerializeField] private List<TerrainData> walls;
        [SerializeField] private List<TerrainData> floors;
        [SerializeField] private FeatureType altarFeature = FeatureType.None;

        [SerializeField] private List<Species> species;

        // Properties
        public string DisplayName { get => displayName; }
        public string RefName { get => refName; }
        public Sprite Icon { get => icon; }

        public FeatureType AltarFeature { get => altarFeature; }

        public override string ToString() => $"{refName}";
    }
}

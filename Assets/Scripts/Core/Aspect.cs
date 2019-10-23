// Aspect.cs
// Jerome Martina

using Pantheon.Actors;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Core
{
    [CreateAssetMenu(fileName = "New Aspect", menuName = "Pantheon/Aspect")]
    public sealed class Aspect : ScriptableObject
    {
        [SerializeField] private string displayName = "NO_ASPECT_NAME";
        [SerializeField] private string refName = "NO_ASPECT_REF";
        [SerializeField] private Sprite icon = null;

        [SerializeField] private List<TerrainData> walls;
        [SerializeField] private List<TerrainData> floors;
        [SerializeField] private FeatureType altarFeature = FeatureType.None;

        [SerializeField] private List<Species> species;
        [SerializeField] private List<Occupation> occupations;

        // Properties
        public string DisplayName { get => displayName; }
        public string RefName { get => refName; }
        public Sprite Icon { get => icon; }

        public FeatureType AltarFeature { get => altarFeature; }
        public List<Species> Species
        {
            get => species;
            private set => species = value;
        }
        public List<Occupation> Occupations
        {
            get => occupations;
            private set => occupations = value;
        }

        public override string ToString() => $"{refName}";
    }
}

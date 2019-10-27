// Aspect.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Core
{
    [CreateAssetMenu(fileName = "New Aspect", menuName = "Pantheon/Aspect")]
    public sealed class Aspect : Content
    {
        [SerializeField] private string displayName = "NO_ASPECT_NAME";
        [SerializeField] private Sprite icon = null;

        [SerializeField] private List<TerrainDef> walls;
        [SerializeField] private List<TerrainDef> floors;
        [SerializeField] private FeatureDef altarFeature = default;

        [SerializeField] private List<Species> species;
        [SerializeField] private List<Occupation> occupations;

        // Properties
        public string DisplayName { get => displayName; }
        public Sprite Icon { get => icon; }

        public FeatureDef AltarFeature { get => altarFeature; }
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

        public override string ToString() => $"{id}";
    }
}

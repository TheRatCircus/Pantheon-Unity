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
        [SerializeField] private AspectID id = AspectID.Default;
        [SerializeField] private Sprite icon = null;

        [SerializeField] private List<TerrainDef> walls;
        [SerializeField] private List<TerrainDef> floors;
        [SerializeField] private FeatureID altarFeature = FeatureID.Default;

        [SerializeField] private List<Species> species;
        [SerializeField] private List<Occupation> occupations;

        // Properties
        public string DisplayName { get => displayName; }
        public AspectID ID { get => id; }
        public Sprite Icon { get => icon; }

        public FeatureID AltarFeature { get => altarFeature; }
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

    public enum AspectID
    {
        Default,
        Fire,
        Greed,
        Swine,
        War
    }
}

// Species.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Prepackaged appendage, natural defense, and trait data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Species", menuName = "BaseData/Species")]
    public class Species : ScriptableObject
    {
        [SerializeField] private string displayName = "NO_NAME";
        [SerializeField] private SpeciesRef reference = SpeciesRef.None;
        [SerializeField] private Sprite sprite = null;
        [SerializeField] private List<BodyPartData> parts = null;
        [SerializeField] private Defenses defenses = null;

        // Properties
        public string DisplayName { get => displayName; }
        public SpeciesRef Reference { get => reference; }
        public Sprite Sprite { get => sprite; }
        public List<BodyPartData> Parts { get => parts; }
        public Defenses Defenses { get => defenses; set => defenses = value; }
    }

    public enum SpeciesRef
    {
        None,
        Human,
        Swine,
        Goose,
        DreadHamster,
        Pigman,
        Coyote,
        GiantBumblebee
    }
}



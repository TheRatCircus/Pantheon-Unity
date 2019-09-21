// Species.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actors
{
    /// <summary>
    /// Prepackaged appendage and trait data for procedurally generating creatures.
    /// </summary>
    [CreateAssetMenu(fileName = "New Species", menuName = "BaseData/Species")]
    public class Species : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private SpeciesRef reference;
        [SerializeField] private Sprite sprite;
        [SerializeField] private List<BodyPartData> parts;

        // Properties
        public string DisplayName { get => displayName; }
        public SpeciesRef Reference { get => reference; }
        public Sprite Sprite { get => sprite; }
        public List<BodyPartData> Parts { get => parts; }
    }

    public enum SpeciesRef
    {
        None,
        Human,
        Swine,
        Goblin,
        DreadHamster,
        Pigman
    }
}



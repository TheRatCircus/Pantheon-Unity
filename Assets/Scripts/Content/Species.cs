// Species.cs
// Jerome Martina

using Pantheon.Actors;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Prepackaged appendage, natural defense, and trait data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Species",
        menuName = "Pantheon/Content/Species")]
    public class Species : Content
    {
        [SerializeField] private string displayName = "NO_NAME";
        [SerializeField] private Sprite sprite = null;
        [SerializeField] private List<AppendageDef> parts = null;
        [SerializeField] private Defenses defenses = null;

        // Properties
        public string DisplayName { get => displayName; }
        public Sprite Sprite { get => sprite; }
        public List<AppendageDef> Appendages { get => parts; }
        public Defenses Defenses { get => defenses; }
    }
}

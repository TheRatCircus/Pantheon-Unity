// Occupation.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Represents a sentient NPC's role in its organization.
    /// </summary>
    [CreateAssetMenu(fileName = "New Occupation",
        menuName = "Pantheon/Content/Occupation")]
    public class Occupation : Content
    {
        [SerializeField] private string displayName = "DEFAULT_OCC_NAME";
        [SerializeField] private List<ItemDef> gear = null;
        [SerializeField] private List<Spell> spells;

        // Properties
        public string DisplayName { get => displayName; }
        public List<ItemDef> Gear { get => gear; }
        public List<Spell> Spells { get => spells; set => spells = value; }
    }
}

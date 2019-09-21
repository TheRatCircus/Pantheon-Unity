// Occupation.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actors
{
    /// <summary>
    /// Represents a sentient NPC's role in its organization.
    /// </summary>
    [CreateAssetMenu(fileName = "New Occupation", menuName = "BaseData/Occupation")]
    public class Occupation : ScriptableObject
    {
        [SerializeField] private string displayName = "NO_NAME";
        [SerializeField] private OccupationRef reference = OccupationRef.None;
        [SerializeField] private List<WeaponType> weapons = null;
        [SerializeField] private List<Spell> spells;

        // Properties
        public string DisplayName { get => displayName; }
        public OccupationRef Reference { get => reference; }
        public List<WeaponType> Weapons { get => weapons; }
        public List<Spell> Spells { get => spells; set => spells = value; }
    }

    public enum OccupationRef
    {
        None,
        Axeman,
    }
}

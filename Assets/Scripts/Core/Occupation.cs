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
        [SerializeField] private string displayName;
        [SerializeField] private OccupationRef reference;
        [SerializeField] private List<WeaponType> weapons;
        [SerializeField] private List<Spell> spells;

        // Properties
        public string DisplayName { get => displayName; }
        public OccupationRef Reference { get => reference; }
        public List<WeaponType> Weapons { get => weapons; }
    }

    public enum OccupationRef
    {
        Axeman
    }
}

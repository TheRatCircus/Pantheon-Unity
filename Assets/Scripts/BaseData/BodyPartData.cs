// BodyPartData.cs
// Jerome Martina

using Pantheon.Actors;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Template for a body part.
    /// </summary>
    [CreateAssetMenu(fileName = "New Body Part",
        menuName = "BaseData/Body Part")]
    public class BodyPartData : ScriptableObject
    {
        [SerializeField] private SpeciesRef species = SpeciesRef.None;
        [SerializeField] private BodyPartType type = BodyPartType.None;
        [SerializeField] private string displayName = null;
        [SerializeField] private Sprite sprite = null;

        [SerializeField] private int strength = 0;
        [SerializeField] private int runBonus = 0;

        [SerializeField] private bool prehensile = false;
        [SerializeField] private bool inherentlyDexterous = false;

        [SerializeField] private Melee melee = null;

        #region Properties

        public SpeciesRef Species { get => species; }
        public BodyPartType Type { get => type; }
        public string DisplayName { get => displayName; }
        public Sprite Sprite { get => sprite; }
        public int Strength { get => strength; }
        public int RunBonus { get => runBonus; }
        public bool Prehensile { get => prehensile; }
        public bool CanMelee { get => inherentlyDexterous; }
        public Melee Melee { get => melee; }
        public bool InherentlyDexterous { get => inherentlyDexterous; }

        #endregion
    }

    /// <summary>
    /// Arm, legs, etc...
    /// </summary>
    public enum BodyPartType
    {
        None,
        Head,
        Arm,
        Legs, // Always in a full set
        Teeth,
        Claw
    }
}

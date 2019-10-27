// AppendageDef.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Template for an appendage.
    /// </summary>
    [CreateAssetMenu(fileName = "New Appendage",
        menuName = "Pantheon/Content/Appendage")]
    public sealed class AppendageDef : ScriptableObject
    {
        [SerializeField] private AppendageType type = AppendageType.None;
        [SerializeField] private string displayName = "DEFAULT_APPENDAGE_NAME";
        [SerializeField] private Sprite sprite = null;

        [SerializeField] private int strength = -1;
        [SerializeField] private int moveTime = -1;

        [SerializeField] private bool prehensile = false;
        [SerializeField] private bool inherentlyDexterous = false;

        [SerializeField] private Melee melee = null;

        #region Properties

        public AppendageType Type { get => type; }
        public string DisplayName { get => displayName; }
        public Sprite Sprite { get => sprite; }
        public int Strength { get => strength; }
        public int MoveTime { get => moveTime; }
        public bool Prehensile { get => prehensile; }
        public bool CanMelee { get => inherentlyDexterous; }
        public Melee Melee { get => melee; }
        public bool InherentlyDexterous { get => inherentlyDexterous; }

        #endregion
    }

    /// <summary>
    /// Arm, legs, etc...
    /// </summary>
    public enum AppendageType
    {
        None,
        Head,
        Arm,
        Legs, // Always in a full set
        Claw,
        Wings,
        Stinger
    }
}

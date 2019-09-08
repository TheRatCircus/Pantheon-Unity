// BodyPartData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Template for a body part.
/// </summary>
[CreateAssetMenu(fileName = "New Body Part", menuName = "Body Part")]
public class BodyPartData : ScriptableObject
{
    [SerializeField] private Species species = Species.None;
    [SerializeField] private BodyPartType type = BodyPartType.None;
    [SerializeField] private string displayName = null;
    [SerializeField] private Sprite sprite = null;

    [SerializeField] private int strength = 0;
    [SerializeField] private int dexterity = 0;
    [SerializeField] private int runBonus = 0;

    [SerializeField] private bool prehensile = false;
    [SerializeField] private bool canMelee = false;

    [SerializeField] private Melee melee = null;

    #region Properties

    public Species Species { get => species; }
    public BodyPartType Type { get => type; }
    public string DisplayName { get => displayName; }
    public Sprite Sprite { get => sprite; }
    public int Strength { get => strength; }
    public int Dexterity { get => dexterity; }
    public int RunBonus { get => runBonus; }
    public bool Prehensile { get => prehensile; }
    public bool CanMelee { get => canMelee; }
    public Melee Melee { get => melee; }

    #endregion
}

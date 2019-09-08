// BodyPartData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Template for a body part.
/// </summary>
[CreateAssetMenu(fileName = "New Body Part", menuName = "Body Part")]
public class BodyPartData : ScriptableObject
{
    public Species species;
    public BodyPartType type;
    public string displayName;
    public Sprite sprite;

    public int strength;
    public int dexterity;
    public int runBonus;

    public bool prehensile = false;
    public bool canMelee = false;

    public Melee melee;
}

// ItemData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Item template data.
/// </summary>
public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string displayName = "NO_NAME";
    [SerializeField] private string refName = "NO_REF";
    [SerializeField] private int maxStack = -1;
    [SerializeField] private bool stackable = false;
    [SerializeField] private Sprite sprite = null;
    [SerializeField] private bool usable = false;

    [SerializeField] private ActionWrapper onUse = null;
    [SerializeField] private string onUseString = "NO_USE_STRING";

    [SerializeField] private bool infiniteThrow = false;
    [SerializeField] private ActionWrapper onThrow = null;

    [SerializeField] private int strengthReq = -1; // Cumulative strength needed to equip
    [SerializeField] private int maxWieldParts = -1; // In how many parts maximum can this be wielded?

    [SerializeField] private Ranged ranged = null;
    [SerializeField] private Melee melee = null;
    [SerializeField] private Ammo ammo = null;

    #region Properties

    public string DisplayName { get => displayName; }
    public string RefName { get => refName; }
    public int MaxStack { get => maxStack; }
    public bool Stackable { get => stackable; }
    public Sprite Sprite { get => sprite; }
    public bool Usable { get => usable; }
    public ActionWrapper OnUse { get => onUse; }
    public string OnUseString { get => onUseString; }
    public int StrengthReq { get => strengthReq; }
    public int MaxWieldParts { get => maxWieldParts; }
    public Melee Melee { get => melee; }
    public Ammo Ammo { get => ammo; }
    public bool InfiniteThrow { get => infiniteThrow; }
    public ActionWrapper OnThrow { get => onThrow; }
    public Ranged Ranged { get => ranged; }

    #endregion
}

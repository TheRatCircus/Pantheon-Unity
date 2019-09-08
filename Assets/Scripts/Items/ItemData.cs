// ItemData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Item template data.
/// </summary>
public abstract class ItemData : ScriptableObject
{
    public string displayName = "NO_NAME";
    public string refName = "NO_REF";
    public int maxStack;
    public bool stackable;
    public Sprite sprite;
    public bool usable;

    public ActionWrapper onUse;
    public string onUseString = "NO_USE_STRING";

    public int strengthReq = -1; // Cumulative strength needed to equip
    public int maxWieldParts; // In how many parts maximum can this be wielded?

    public Melee Melee = null;
}

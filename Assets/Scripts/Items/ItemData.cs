// A template holding the data by which items define themselves
using UnityEngine;

public enum ItemFamily
{
    Potion,
    Corpse
}

public abstract class ItemData : ScriptableObject
{
    public string displayName;
    public string refName;
    public int maxStack;
    public bool stackable;
    public Sprite sprite;
    public bool usable;

    // Effect activated when this item is used
    public abstract void OnUse(Actor user);
}

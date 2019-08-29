// A template holding the data by which items define themselves
using UnityEngine;
using Pantheon.Actions;

public abstract class ItemData : ScriptableObject
{
    public string displayName;
    public string refName;
    public int maxStack;
    public bool stackable;
    public Sprite sprite;
    public bool usable;

    public BaseAction onUse;
    public string onUseString;

    // Effect activated when this item is used
    public abstract void OnUse(Player user, Item item);
    public abstract void OnUse(Enemy user, Item item);
}

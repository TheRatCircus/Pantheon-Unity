// Class representing an actualized item in the game
using UnityEngine;
using Pantheon.Actions;

public class Item
{
    // Item's own attributes
    private Actor owner;
    private int quantity;
    private ItemData itemData;

    // Properties of self
    public Actor Owner { get => owner; set => owner = value; }

    // Properties for accessing ItemData
    public string DisplayName { get => itemData.displayName; }
    public string RefName { get => itemData.refName; }
    public int MaxStack { get => itemData.maxStack; }
    public bool Stackable { get => itemData.stackable; }
    public Sprite _sprite { get => itemData.sprite; }
    public bool Usable { get => itemData.usable; }

    public string OnUseString { get => itemData.onUseString; }
    public BaseAction OnUse { get => itemData.onUse; }

    // Constructor
    public Item(ItemData itemData)
    {
        this.itemData = itemData;
    }

    // Use this item
    public void Use(Actor user)
    {
        if (user is Player)
            itemData.OnUse((Player)user, this);
        else
            itemData.OnUse((Enemy)user, this);
    }
}

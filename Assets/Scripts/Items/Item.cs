// Class representing an actualized item in the game
using UnityEngine;
using Pantheon.Actions;

public class Item
{
    // Item's own attributes
    private string displayName;
    private Sprite sprite;
    private Actor owner;
    private bool stackable;
    private int quantity;
    private int weight;

    private ActionWrapper onUse;

    // Properties
    public Actor Owner { get => owner; set => owner = value; }
    public string DisplayName { get => displayName; }
    public Sprite _sprite { get => sprite; }
    public ActionWrapper OnUse { get => onUse; }

    // Constructor
    public Item(ItemData itemData)
    {
        displayName = itemData.displayName;
        sprite = itemData.sprite;
        stackable = itemData.stackable;

        onUse = itemData.onUse;

        owner = null;
    }

    // Use this item
    public void Use(Actor user)
    {
        if (onUse == null)
        {
            if (user is Player)
                GameLog.Send("This item cannot be used.", MessageColour.Grey);
            else
                Debug.LogWarning("An NPC tried to use an unusable item.");

            return;
        }
        
        ItemUseAction use = new ItemUseAction(user, this, onUse.GetAction(user));
    }
}

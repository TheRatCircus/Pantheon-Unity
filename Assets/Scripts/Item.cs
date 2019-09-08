// Item.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.Actions;

/// <summary>
/// An actualized item.
/// </summary>
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
    private Melee melee;
    private int strengthReq;
    private int maxWieldParts;

    // Items save the last parts they were wielded in, and are wielded there
    // again next time they are equipped
    private BodyPart[] wieldProfile;

    // Properties
    public Actor Owner { get => owner; set => owner = value; }
    public string DisplayName { get => displayName; }
    public Sprite _sprite { get => sprite; }
    public ActionWrapper OnUse { get => onUse; }
    public int StrengthReq { get => strengthReq; }
    public int MaxWieldParts { get => maxWieldParts; }
    public BodyPart[] WieldProfile { get => wieldProfile; set => wieldProfile = value; }

    // Constructor
    public Item(ItemData itemData)
    {
        displayName = itemData.displayName;
        sprite = itemData.sprite;
        stackable = itemData.stackable;

        onUse = itemData.onUse;
        melee = itemData.Melee;
        strengthReq = itemData.strengthReq;
        maxWieldParts = itemData.maxWieldParts;

        owner = null;
    }

    // Use this item
    public void Use(Actor user)
    {
        if (onUse == null)
        {
            TryEquip(user);
            return;
        }

        ItemUseAction use = new ItemUseAction(user, this, onUse.GetAction(user));
    }

    public void TryEquip(Actor user)
    {
        if (user is Enemy)
            throw new System.NotImplementedException("NPC attempted to wield.");

        if (!user.HasPrehensile())
        {
            GameLog.Send("You have no way of grasping this.", MessageColour.Orange);
            return;
        }

        // Present modal dialog, let user choose parts to hold item
    }
}

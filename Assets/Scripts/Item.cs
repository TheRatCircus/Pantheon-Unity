// Item.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.Actions;
using Pantheon.Utils;

public class Item
{
    private bool stackable;
    private int quantity;
    private int weight;

    // Properties
    public Actor Owner { get; set; }
    public string DisplayName { get; }
    public Sprite Sprite { get; }
    public ActionWrapper OnUse { get; }
    public int StrengthReq { get; }
    public int MaxWieldParts { get; }
    // Items save the last parts they were wielded in, and are wielded there
    // again next time they are equipped
    public BodyPart[] WieldProfile { get; set; }
    public Melee Melee { get; set; }

    // Constructor
    public Item(ItemData itemData)
    {
        DisplayName = itemData.DisplayName;
        Sprite = itemData.Sprite;
        stackable = itemData.Stackable;

        OnUse = itemData.OnUse;
        Melee = itemData.Melee;
        StrengthReq = itemData.StrengthReq;
        MaxWieldParts = itemData.MaxWieldParts;

        Owner = null;
    }

    /// <summary>
    /// Corpse constructor.
    /// </summary>
    /// <param name="actor">Actor off which to base the new corpse.</param>
    public Item(Actor actor)
    {
        DisplayName = $"{actor.ActorName} corpse";
        Sprite = actor.CorpseSprite;
        // TODO: Derive stats from actor weight
    }

    // Use this item
    public void Use(Actor user)
    {
        if (OnUse == null)
        {
            TryEquip(user);
            return;
        }

        ItemUseAction use = new ItemUseAction(user, this, OnUse.GetAction(user));
    }

    public void TryEquip(Actor user)
    {
        if (user is NPC)
            throw new System.NotImplementedException("NPC attempted to wield.");

        if (!user.HasPrehensile())
        {
            GameLog.Send("You have no way of grasping this.",
                Strings.TextColour.Orange);
            return;
        }

        // Present modal dialog, let user choose parts to hold item
    }

    public override string ToString()
        => DisplayName;
}

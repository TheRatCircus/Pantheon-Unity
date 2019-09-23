// Item.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.Actions;
using Pantheon.Utils;

public sealed class Item
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
    public Ranged Ranged { get; set; }
    public bool IsRanged => Ranged.MaxDamage >= 0;

    public ActionWrapper OnToss { get; }
    public bool InfiniteThrow { get; set; }

    public Ammo Ammo { get; set; }
    public bool IsAmmo => Ammo.MaxDamage >= 0;

    public Defenses Defenses { get; set; }

    public EquipType EquipType { get; set; }

    public Item(ItemData itemData)
    {
        DisplayName = itemData.DisplayName;
        Sprite = itemData.Sprite;
        
        stackable = itemData.Stackable;

        OnUse = itemData.OnUse;

        Melee = itemData.Melee;
        if (itemData.IsRanged)
            Ranged = itemData.Ranged;
        if (itemData.IsAmmo)
            Ammo = itemData.Ammo;

        StrengthReq = itemData.StrengthReq;
        MaxWieldParts = itemData.MaxWieldParts;

        OnToss = itemData.OnThrow;
        InfiniteThrow = itemData.InfiniteThrow;

        if (itemData.HasDefenses)
            Defenses = itemData.Defenses;

        EquipType = itemData.EquipType;

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
        if (OnUse != null)
        {
            ItemUseAction use
                = new ItemUseAction(user, this, OnUse.GetAction(user));
        }
        else if (EquipType != EquipType.None)
        {
            TryWear(user);
        }
    }

    public void TryWield(Actor user)
    {
        if (user is NPC)
            throw new System.NotImplementedException("NPC attempted to wield.");

        if (!user.Body.HasPrehensile())
        {
            GameLog.Send("You have no way of grasping this.",
                Strings.TextColour.Orange);
            return;
        }

        // Present modal dialog, let user choose parts to hold item
    }

    public void TryWear(Actor user)
    {
        if (user is NPC)
            throw new System.NotImplementedException("NPC attempted to wear.");

        user.NextAction = new WearAction(user, this);
    }

    public override string ToString()
        => DisplayName;
}

[System.Serializable]
public sealed class Defenses
{
    [SerializeField] private int armour;
    [SerializeField] private int evasion;

    // Resists run from -1.0 to 1.0
    [SerializeField] private float resistPhys;
    [SerializeField] private float resistHeat;
    [SerializeField] private float resistCold;
    [SerializeField] private float resistDisease;
    [SerializeField] private float resistPoison;

    public int Armour { get => armour; set => armour = value; }
    public int Evasion { get => evasion; set => evasion = value; }
}

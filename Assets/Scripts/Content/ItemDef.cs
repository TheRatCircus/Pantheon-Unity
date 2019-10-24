// ItemDef.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;
using System.Collections.Generic;

namespace Pantheon
{
    /// <summary>
    /// Base item type definition.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Pantheon/Content/Item")]
    public sealed class ItemDef : ScriptableObject
    {
        [SerializeField] private string displayName = "DEFAULT_ITEM_NAME";
        [SerializeField] private ItemID id = ItemID.Default;

        [SerializeField] private int maxStack = -1;
        [SerializeField] private Sprite sprite = null;

        [SerializeField] private ActionWrapper onUse = null;
        [SerializeField] private string onUseString = "NO_USE_STRING";

        [SerializeField] private bool destroyedOnToss = false;
        [SerializeField] private bool infiniteThrow = false;
        [SerializeField] private ActionWrapper onThrow = null;

        // Cumulative strength needed to equip
        [SerializeField] private int strengthReq = -1;
        [SerializeField] private int maxWieldParts = -1;

        [SerializeField] private Melee melee = null;
        [SerializeField]
        private List<ComponentWrapper> components
            = new List<ComponentWrapper>();

        #region Properties

        public string DisplayName => displayName;
        public ItemID ID => id;

        public int MaxStack => maxStack;
        public Sprite Sprite => sprite;

        public ActionWrapper OnUse => onUse;
        public string OnUseString => onUseString;

        public int StrengthReq => strengthReq;
        public int MaxWieldAppendages => maxWieldParts;

        public bool DestroyedOnToss => destroyedOnToss;
        public bool InfiniteThrow => infiniteThrow;
        public ActionWrapper OnThrow => onThrow;

        public Melee Melee => melee;
        public List<ComponentWrapper> Components => components;

        #endregion
    }

    public enum ItemCategory
    {
        Weapon,
        Ammo,
        Wearable,
        Consumable,
        Misc
    }

    public enum ItemID
    {
        Default,
        Dagger,
        Hatchet,
        Prejudice,
        Carbine,
        Flask,
        Cartridges,
        Cuirass,
        HandGrenade
    }
}

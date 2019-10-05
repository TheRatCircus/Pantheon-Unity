// ItemData.cs
// Jerome Martina

using Pantheon.Components;
using UnityEngine;
using System.Collections.Generic;

namespace Pantheon
{
    /// <summary>
    /// Item template data.
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        [SerializeField] private string displayName = "NO_NAME";
        [SerializeField] private int maxStack = -1;
        [SerializeField] private bool stackable = false;
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
        public int MaxStack => maxStack;
        public bool Stackable => stackable;
        public Sprite Sprite => sprite;

        public ActionWrapper OnUse => onUse;
        public string OnUseString => onUseString;

        public int StrengthReq => strengthReq;
        public int MaxWieldParts => maxWieldParts;

        public bool DestroyedOnToss => destroyedOnToss;
        public bool InfiniteThrow => infiniteThrow;
        public ActionWrapper OnThrow => onThrow;

        public Melee Melee => melee;
        public List<ComponentWrapper> Components => components;

        #endregion
    }
}

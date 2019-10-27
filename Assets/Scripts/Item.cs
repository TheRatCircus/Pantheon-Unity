// Item.cs
// Jerome Martina

using Pantheon.Actions;
using Pantheon.Actors;
using Pantheon.Components;
using Pantheon.Core;
using Pantheon.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    public sealed class Item : IDescribable
    {
        public string DisplayName { get; set; }
        public string BaseName { get; private set; }
        public Sprite Sprite { get; }
        
        private int weight;

        public Dictionary<ComponentType, IComponent> Components
        { get; private set; }
            = new Dictionary<ComponentType, IComponent>();
        public ActionWrapper OnUse { get; }
        public List<Enchant> Enchants { get; set; } = new List<Enchant>();

        public int MaxStack { get; set; }
        public bool Stackable => MaxStack > 1;
        //private int quantity;
        public Actor Owner { get; set; } = null;
        
        public int StrengthReq { get; }
        public int MaxWieldAppendages { get; }
        // Items save the last parts they were wielded in, and are wielded
        // there again next time they are equipped
        public Appendage[] WieldProfile { get; set; }

        public Melee Melee { get; set; }

        public ActionWrapper OnToss { get; }
        public bool DestroyedOnToss { get; set; }
        public bool InfiniteToss { get; set; }
        public bool ReturnsOnToss { get; set; }

        public bool HasComponent(ComponentType type)
            => Components.ContainsKey(type);

        // Properties for fast component checks
        public bool IsRanged => HasComponent(ComponentType.Ranged);
        public bool IsAmmo => HasComponent(ComponentType.Ammo);

        public T GetComponent<T>() where T : class
        {
            foreach (KeyValuePair<ComponentType, IComponent> pair
                in Components)
            {
                if (pair.Value is T t)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Factory function to shorten calls.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item NewItem(string id)
        {
            return new Item(Database.Get<ItemDef>(id));
        }

        public Item(ItemDef itemDef)
        {
            DisplayName = BaseName = itemDef.DisplayName;
            Sprite = itemDef.Sprite;
            MaxStack = itemDef.MaxStack;

            OnUse = itemDef.OnUse;

            Melee = itemDef.Melee;

            StrengthReq = itemDef.StrengthReq;
            MaxWieldAppendages = itemDef.MaxWieldAppendages;

            DestroyedOnToss = itemDef.DestroyedOnToss;
            OnToss = itemDef.OnThrow;
            InfiniteToss = itemDef.InfiniteThrow;

            foreach (ComponentWrapper c in itemDef.Components)
            {
                Components.Add(c.Get.Type, c.Get);
            }
        }

        /// <summary>
        /// Corpse constructor.
        /// </summary>
        /// <param name="actor">Actor off which to base the new corpse.</param>
        public Item(Actor actor)
        {
            DisplayName = $"{actor.Species.DisplayName} corpse";
            Sprite = actor.CorpseSprite;
            Components.Add(ComponentType.Corpse, new Corpse(actor));
            // TODO: Derive stats from actor weight
        }

        public void Use(Actor user)
        {
            if (OnUse != null)
            {
                ItemUseAction use
                    = new ItemUseAction(user, this, OnUse.GetAction(user));
            }
            else if (Components.ContainsKey(ComponentType.Equipment))
            {
                TryWear(user);
            }
        }

        public void TryWield(Actor user)
        {
            if (user is NPC)
                throw new System.NotImplementedException
                    ("NPC attempted to wield.");

            if (!user.Body.HasPrehensile())
            {
                GameLog.Send("You have no way of grasping this.",
                    Strings.TextColour.Orange);
                return;
            }

            // Present modal dialog, let user choose appendages to hold item
        }

        public void TryWear(Actor user)
        {
            if (user is NPC)
                throw new System.NotImplementedException
                    ("NPC attempted to wear.");

            user.NextAction = new WearAction(user, this);
        }

        public void OnEquip(Actor equipper)
        {
            foreach (Enchant e in Enchants)
            {
                if (e is IOnEquipEnchant equipEnchant)
                    equipEnchant.EquipEffect(equipper);
            }
        }

        public override string ToString()
            => DisplayName;
    }
}

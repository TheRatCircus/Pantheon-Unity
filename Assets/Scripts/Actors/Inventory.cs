// Inventory.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Actors
{
    [Serializable]
    public sealed class Inventory
    {
        [SerializeField] [ReadOnly] private List<Item> all;
        [SerializeField] [ReadOnly] private List<Item> wielded = new List<Item>();

        public List<Item> All { get => all; }
        public List<Item> Wielded { get => wielded; }

        public Inventory(int inventorySize)
            => all = new List<Item>(inventorySize);

        public void AddItem(Item item)
            => all.Add(item);
        public bool RemoveItem(Item item)
            => all.Remove(item);

        public bool HasAmmoFor(Item rangedWeapon)
        {
            if (!rangedWeapon.IsRanged)
                throw new ArgumentException
                    ("Argument item must be a ranged weapon.");

            foreach (Item item in all)
                if (item.IsAmmo && item.Ammo.AmmoFamily
                    == rangedWeapon.Ranged.AmmoFamily)
                    return true;

            return false;
        }

        public bool WieldingRangedWeapon()
        {
            foreach (Item item in wielded)
                if (item.IsRanged)
                    return true;

            return false;
        }
    }
}
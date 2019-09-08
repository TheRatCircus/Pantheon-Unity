// A slot in the inventory GUI
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public class InventorySlot : MonoBehaviour
    {
        public Image icon; // Image displaying the item's sprite

        private Item item; // Item referred to by this slot

        // Events
        public event Action<string> OnHoverEvent;
        public event Action<Item> OnUseEvent;
        public event Action<Item> OnDropEvent;

        // Add item to slot on inventory UI update
        public void AddItem(Item item)
        {
            this.item = item;

            icon.sprite = item._sprite;
            icon.enabled = true;
        }

        // Clear slot on inventory UI update
        public void ClearSlot()
        {
            item = null;

            icon.sprite = null;
            icon.enabled = false;
        }

        // Use this item by pressing button
        public void UseItem()
        {
            if (item != null)
                OnUseEvent?.Invoke(item);
        }

        public void DropItem()
        {
            if (item != null)
                OnDropEvent?.Invoke(item);
        }

        // Action to take when pointer enters button
        public void OnPointerEnter()
        {
            if (item != null)
                OnHoverEvent?.Invoke(item.DisplayName);
        }

        // Action to take when pointer exits button
        public void OnPointerExit()
        {
            OnHoverEvent?.Invoke("");
        }
    }
}
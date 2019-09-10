// InventorySlot.cs
// Jerome Martina

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image icon = null; // Image displaying the item's sprite

        [SerializeField] [ReadOnly] private Item item; // Item referred to by this slot

        // Events
        public event Action<string> OnHoverEvent;
        public event Action<Item> OnUseEvent;
        public event Action<Item> OnDropEvent;

        // Add item to slot on inventory UI update
        public void AddItem(Item item)
        {
            this.item = item;

            icon.sprite = item.Sprite;
            icon.enabled = true;
        }

        // Clear slot on inventory UI update
        public void ClearSlot()
        {
            item = null;

            icon.sprite = null;
            icon.enabled = false;
        }

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

        public void OnPointerEnter()
        {
            if (item != null)
                OnHoverEvent?.Invoke(item.DisplayName);
        }

        public void OnPointerExit()
        {
            OnHoverEvent?.Invoke("");
        }
    }
}
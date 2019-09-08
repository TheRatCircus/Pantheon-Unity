// Inventory GUI controller

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Actors;
using Pantheon.Actions;

namespace Pantheon.UI
{
    public class InventoryUI : MonoBehaviour
    {
        // Requisite objects
        public Transform inventoryGrid;
        public GameObject inventoryUI;
        public Player player;
        public GameObject slotPrefab;
        public Text itemNameText;

        // The inventory slots the GUI holds
        private List<InventorySlot> inventorySlots;

        // Start is called before the first frame update
        private void Start()
        {
            // Instantiate slot prefabs and add their scripts to inventorySlots
            inventorySlots = new List<InventorySlot>();
            for (int i = 0; i < player.InventorySize; i++)
            {
                GameObject newSlotObj = Instantiate(slotPrefab, inventoryGrid, false);
                InventorySlot newSlot = newSlotObj.GetComponent<InventorySlot>();
                inventorySlots.Add(newSlot);
                newSlot.OnHoverEvent += UpdateItemName;
                newSlot.OnUseEvent += UseItem;
                newSlot.OnDropEvent += DropItem;
            }
            player.OnInventoryChangeEvent += UpdateInventory;
            player.OnInventoryToggleEvent += ToggleInventory;
        }

        // Open and close the inventory
        private void ToggleInventory()
            => inventoryUI.SetActive(!inventoryUI.activeInHierarchy);

        // Update slots to reflect inventory
        private void UpdateInventory()
        {
            for (int i = 0; i < player.InventorySize; i++)
            {
                if (i < player.Inventory.Count)
                    inventorySlots[i].AddItem(player.Inventory[i]);
                else
                    inventorySlots[i].ClearSlot();
            }
        }

        // Update the displayed name of the hovered item
        private void UpdateItemName(string itemName)
            => itemNameText.text = itemName;

        // Send item usage call to the player
        private void UseItem(Item item)
            => item.Use(player);

        // Send item drop call to the player
        private void DropItem(Item item)
            => player.nextAction = new DropAction(player, item);
    }
}

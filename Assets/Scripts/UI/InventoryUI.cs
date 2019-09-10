// InventoryUI.cs
// Jerome Martina
// Credit to Asbjørn "Brackeys" Thorslund

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
        [SerializeField] private Transform inventoryGrid = null;
        [SerializeField] private GameObject inventoryUI = null;
        [SerializeField] private Player player = null;
        [SerializeField] private GameObject slotPrefab = null;
        [SerializeField] private Text itemNameText = null;

        // The inventory slots the GUI holds
        [SerializeField] [ReadOnly] private List<InventorySlot> inventorySlots
            = new List<InventorySlot>();

        // Start is called before the first frame update
        private void Start()
        {
            // Instantiate slot prefabs and add their scripts to inventorySlots
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

        private void ToggleInventory()
            => inventoryUI.SetActive(!inventoryUI.activeInHierarchy);

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
            => player.NextAction = new DropAction(player, item);
    }
}

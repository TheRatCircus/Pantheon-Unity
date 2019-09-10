// HUD.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Actions;

namespace Pantheon.UI
{
    public class HUD : MonoBehaviour
    {
        // Player
        public Player player;

        // UI elements
        public Text healthCounter;
        public Text energyCounter;
        public Text turnCounter;
        public Text locationDisplay;

        // Modals
        public ItemModalList itemModalList;
        public BodyPartModalList bodyPartModalList;

        // Start is called before the first frame update
        private void Start()
        {
            player.OnHealthChangeEvent += UpdateHealthCounter;
            player.Input.ModalListOpenEvent += OpenModalList;
            player.Input.ModalCancelEvent += ClearModals;
            Game.instance.OnPlayerActionEvent += UpdateEnergyCounter;
            Game.instance.OnTurnChangeEvent += UpdateTurnCounter;
            Game.instance.OnLevelChangeEvent += UpdateLocationDisplay;

            UpdateTurnCounter(0);
            UpdateHealthCounter(player.Health, player.MaxHealth);
            UpdateEnergyCounter(player.Energy);
        }

        private void UpdateHealthCounter(int health, int maxHealth)
        {
            string healthCounterStr = $"Health: {health} / {maxHealth}";
            healthCounter.text = healthCounterStr;
        }

        private void UpdateEnergyCounter(int energy)
        {
            string energyCounterStr = $"Energy: {energy}";
            energyCounter.text = energyCounterStr;
        }

        private void UpdateTurnCounter(int turn)
        {
            string turnCounterStr = $"Time: {turn}";
            turnCounter.text = turnCounterStr;
        }

        private void UpdateLocationDisplay(Level level)
        {
            string locationDisplayStr = $"Location: {level.DisplayName}";
            locationDisplay.text = locationDisplayStr;
        }

        private void ClearModals()
        {
            itemModalList.Clean();
            itemModalList.gameObject.SetActive(false);
            bodyPartModalList.Clean();
            bodyPartModalList.gameObject.SetActive(false);
        }

        private void OpenModalList(ModalListOperation op)
        {
            switch (op)
            {
                case ModalListOperation.Wield:
                    ItemWieldModalList();
                    break;
                default:
                    throw new System.Exception("No modal list operation given.");
            }
        }

        private void ItemWieldModalList()
        {
            ClearModals();
            itemModalList.gameObject.SetActive(true);
            itemModalList.Initialize("Wield which item?", player, 1,
                WieldPartsModalList);
        }

        private void WieldPartsModalList(Item item)
        {
            ClearModals();
            bodyPartModalList.gameObject.SetActive(true);
            bodyPartModalList.Initialize(
                $"Wield where? Select up to {item.MaxWieldParts}.",
                player, item.MaxWieldParts,
                (List<BodyPart> parts) =>
                {
                    ClearModals();
                    player.Input.SetInputState(InputState.Move);
                    player.NextAction = new WieldAction(player, item, parts);
                });
            player.Input.ModalConfirmEvent += bodyPartModalList.Submit;
        }
    }
}

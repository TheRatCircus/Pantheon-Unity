// HUD.cs
// Jerome Martina

#define DEBUG_MODAL
#undef DEBUG_MODAL

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Actions;
using Pantheon.Utils;

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
        public Text statusDisplay;

        // UI element backends
        public List<string> statusNames;

        // Modals
        public ItemModalList itemModalList;
        public BodyPartModalList bodyPartModalList;
        public SpellModalList spellModalList;

        // Start is called before the first frame update
        private void Start()
        {
            player.OnHealthChangeEvent += UpdateHealthCounter;
            player.Input.ModalListOpenEvent += OpenModalList;
            player.Input.ModalCancelEvent += ClearModals;
            player.StatusChangeEvent += UpdateStatuses;
            Game.instance.OnPlayerActionEvent += UpdateEnergyCounter;
            Game.instance.OnClockTickEvent += UpdateTurnCounter;
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

        private void UpdateStatuses(List<StatusEffect> statuses)
        {
            statusDisplay.text = "";

            foreach (StatusEffect status in statuses)
            {
                string displayName = Strings.ColourString(status.DisplayName,
                    status.LabelColour);
                statusDisplay.text += $"{displayName} ";
            }
        }

        private void ClearModals()
        {
            LogClearModals();
            itemModalList.Clean();
            itemModalList.gameObject.SetActive(false);
            bodyPartModalList.Clean();
            bodyPartModalList.gameObject.SetActive(false);
            spellModalList.Clean();
            spellModalList.gameObject.SetActive(false);
            player.Input.ModalCancelEvent += ClearModals;
        }

        [System.Diagnostics.Conditional("DEBUG_MODAL")]
        private void LogClearModals()
        {
            Debug.Log("Clearing modal dialogs...");
        }

        private void OpenModalList(ModalListOperation op)
        {
            switch (op)
            {
                case ModalListOperation.Wield:
                    ItemWieldModalList();
                    break;
                case ModalListOperation.Spell:
                    SpellModalList();
                    break;
                default:
                    throw new System.Exception("No modal list operation given.");
            }
        }

        private void SpellModalList()
        {
            ClearModals();
            spellModalList.gameObject.SetActive(true);
            spellModalList.Initialize("Cast which spell?", player, 1,
                (Spell spell) =>
                {
                    ClearModals();
                    player.Input.SetInputState(InputState.Move);
                    player.NextAction = new CastAction(player, spell);
                }
                );
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

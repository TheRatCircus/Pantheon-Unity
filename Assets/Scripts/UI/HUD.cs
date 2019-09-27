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
        [SerializeField] private Text healthCounter = null;
        [SerializeField] private Text energyCounter = null;
        [SerializeField] private Text turnCounter = null;
        [SerializeField] private Text locationDisplay = null;
        [SerializeField] private Text armourCounter = null;
        [SerializeField] private Text evasionCounter = null;
        [SerializeField] private Text statusDisplay = null;
        [SerializeField] private GameObject worldMap = null;
        [SerializeField] private GameObject traitMenu = null;

        // Modals
        public ItemModalList itemModalList;
        public BodyPartModalList bodyPartModalList;
        public SpellModalList spellModalList;

        // Start is called before the first frame update
        private void Start()
        {
            traitMenu.GetComponent<TraitMenu>().Initialize(player);

            player.OnHealthChangeEvent += UpdateHealthCounter;
            player.Input.WorldMapToggleEvent += ToggleWorldMap;
            player.Input.ModalListOpenEvent += OpenModalList;
            player.Input.ModalCancelEvent += ClearModals;
            player.Input.TraitMenuToggleEvent += ToggleTraitMenu;
            player.StatusChangeEvent += UpdateStatuses;
            player.Defenses.RecalculateEvent += UpdateDefenses;
            Game.instance.OnPlayerActionEvent += UpdateEnergyCounter;
            Game.instance.OnClockTickEvent += UpdateTurnCounter;
            Game.instance.OnLevelChangeEvent += UpdateLocationDisplay;

            UpdateTurnCounter(0);
            UpdateHealthCounter(player.Health, player.MaxHealth);
            UpdateEnergyCounter(player.Energy);
            UpdateDefenses(player.Defenses);
            UpdateLocationDisplay(player.level);
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

        private void UpdateDefenses(Actors.Defenses defenses)
        {
            armourCounter.text = $"Armour: {defenses.Armour}";
            evasionCounter.text = $"Evasion: {defenses.Evasion}";
        }

        private void ToggleWorldMap()
        {
            worldMap.SetActive(!worldMap.activeSelf);
        }

        private void ToggleTraitMenu()
        {
            traitMenu.SetActive(!traitMenu.activeSelf);
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
            UnityEngine.Debug.Log("Clearing modal dialogs...");
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
                case ModalListOperation.Toss:
                    TossModalList();
                    break;
                default:
                    throw new System.Exception("No modal list operation given.");
            }
        }

        private void TossModalList()
        {
            ClearModals();
            itemModalList.gameObject.SetActive(true);
            itemModalList.Initialize("Toss which item?", player, 1,
                (Item item) =>
                {
                    ClearModals();
                    player.Input.SetInputState(InputState.Move);
                    TossAction toss = new TossAction(player, item);
                });
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
                    CastAction cast = new CastAction(player, spell);
                });
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

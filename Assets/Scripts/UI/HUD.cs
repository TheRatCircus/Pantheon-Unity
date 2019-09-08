// Handler for Heads-Up-Display

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        player._input.ModalListOpenEvent += OpenModalList;
        player._input.ModalCancelEvent += ClearModals;
        Game.instance.OnPlayerActionEvent += UpdateEnergyCounter;
        Game.instance.OnTurnChangeEvent += UpdateTurnCounter;
        Game.instance.OnLevelChangeEvent += UpdateLocationDisplay;

        UpdateTurnCounter(0);
        UpdateHealthCounter(player.Health, player.MaxHealth);
        UpdateEnergyCounter(player.energy);
    }

    // Update the health counter
    private void UpdateHealthCounter(int health, int maxHealth)
    {
        string healthCounterStr = $"Health: {health} / {maxHealth}";
        healthCounter.text = healthCounterStr;
    }

    // Update the energy counter
    private void UpdateEnergyCounter(int energy)
    {
        string energyCounterStr = $"Energy: {energy}";
        energyCounter.text = energyCounterStr;
    }

    // Update the turn counter
    private void UpdateTurnCounter(int turn)
    {
        string turnCounterStr = $"Time: {turn}";
        turnCounter.text = turnCounterStr;
    }
    
    // Update the location display
    private void UpdateLocationDisplay(Level level)
    {
        string locationDisplayStr = $"Location: {level.DisplayName}";
        locationDisplay.text = locationDisplayStr;
    }

    private void ClearModals()
    {
        itemModalList.gameObject.SetActive(false);
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
                player._input.InputState = InputState.Move;
                player.nextAction = new WieldAction(player, item, parts);
            });
        player._input.ModalConfirmEvent += bodyPartModalList.Submit;
    }
}

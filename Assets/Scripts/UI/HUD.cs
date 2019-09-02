// Handler for Heads-Up-Display
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

    // Start is called before the first frame update
    private void Start()
    {
        player.OnHealthChangeEvent += UpdateHealthCounter;
        Game.instance.OnPlayerActionEvent += UpdateEnergyCounter;
        Game.instance.OnTurnChangeEvent += UpdateTurnCounter;

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
}

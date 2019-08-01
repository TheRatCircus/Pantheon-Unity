// Handler for Heads-Up-Display
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Player player;

    public Text turnCounter;
    public Text healthCounter;

    private void Start()
    {
        TurnController.instance.OnTurnChangeEvent += UpdateTurnCounter;
        player.OnHealthChangeEvent += UpdateHealthCounter;
        UpdateTurnCounter();
        healthCounter.text = $"Health: {player.Health} / {player.MaxHealth}";
    }

    // Update the turn counter
    private void UpdateTurnCounter()
    {
        string turnCounterStr = $"Turn: {TurnController.instance.turn}";
        turnCounter.text = turnCounterStr;
    }

    // Update the health counter
    private void UpdateHealthCounter(int health, int maxHealth)
    {
        string healthCounterStr = $"Health: {health} / {maxHealth}";
        healthCounter.text = healthCounterStr;
    }
}

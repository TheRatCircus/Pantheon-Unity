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
        UpdateHealthCounter();
    }

    // Update the turn counter
    private void UpdateTurnCounter()
    {
        string turnCounterStr = $"Turn: {TurnController.instance.turn}";
        turnCounter.text = turnCounterStr;
    }

    // Update the health counter
    private void UpdateHealthCounter()
    {
        string healthCounterStr = $"Health: {player.Health}";
        healthCounter.text = healthCounterStr;
    }
}

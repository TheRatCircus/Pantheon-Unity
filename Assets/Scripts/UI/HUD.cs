// Handler for Heads-Up-Display
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Player
    public Player player;

    // UI elements
    public Text turnCounter;
    public Text healthCounter;

    // Start is called before the first frame update
    private void Start()
    {
        Game.instance.OnTurnChangeEvent += UpdateTurnCounter;
        player.OnHealthChangeEvent += UpdateHealthCounter;
        UpdateTurnCounter();
        UpdateHealthCounter(player.Health, player.MaxHealth);
    }

    // Update the turn counter
    private void UpdateTurnCounter()
    {
        string turnCounterStr = $"Turn: {Game.instance.turn}";
        turnCounter.text = turnCounterStr;
    }

    // Update the health counter
    private void UpdateHealthCounter(int health, int maxHealth)
    {
        string healthCounterStr = $"Health: {health} / {maxHealth}";
        healthCounter.text = healthCounterStr;
    }
}

// Handler for Heads-Up-Display
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // UI elements
    public Text turnCounter;
    public Text healthCounter;

    // Start is called before the first frame update
    private void Start()
    {
        TurnController.instance.OnTurnChangeEvent += UpdateTurnCounter;
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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

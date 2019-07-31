// Main turn loop
using UnityEngine;
using System;

// Represents the finite list of states the game can be in
public enum GameState
{
    Player0Turn = 0,
    EnemyTurn = 1,
    PlayersDead = 2
}

public class TurnController : MonoBehaviour
{
    // Singleton
    public static TurnController instance;

    public int turn = 0;

    public GameState gameState = GameState.Player0Turn;

    // Events
    public event Action OnTurnChangeEvent;
    public event Action OnNPCTurnEvent;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        instance = this;
    }

    // Change turn over to next state
    public void ChangeTurn()
    {
        switch (gameState)
        {
            case GameState.Player0Turn:
                gameState = GameState.EnemyTurn;
                DoEnemyTurn();
                break;
            case GameState.EnemyTurn:
                gameState = GameState.Player0Turn;
                // End of round
                OnTurnChangeEvent?.Invoke();
                break;
        }
        turn++;
    }

    // Send event to all enemies allowing them to carry out their turn,
    // and then change back to players
    private void DoEnemyTurn()
    {
        OnNPCTurnEvent?.Invoke();
        ChangeTurn();
    }
}

// Main turn loop
using UnityEngine;
using System;

// Represents the finite list of states the game can be in
public enum GameState
{
    Player0Turn = 0,
    EnemyTurn = 1
}

public class TurnController : MonoBehaviour
{
    // Singleton
    public static TurnController turnController;

    public int round = 0;
    public int turn = 0;

    public GameState gameState = GameState.Player0Turn;

    // Events
    public event Action OnEnemyTurnEvent;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        turnController = this;
    }

    // Change turn over to next state
    public void ChangeTurn()
    {
        switch (gameState)
        {
            case GameState.Player0Turn:
                gameState = GameState.EnemyTurn;
                //Debug.Log("Enemies taking turn.");
                DoEnemyTurn();
                break;
            case GameState.EnemyTurn:
                gameState = GameState.Player0Turn;
                //Debug.Log("Players taking turn");
                round++;
                break;
        }
        turn++;
    }

    // Send event to all enemies allowing them to carry out their turn,
    // and then change back to players
    private void DoEnemyTurn()
    {
        OnEnemyTurnEvent?.Invoke();
        ChangeTurn();
    }
}

// Main turn loop
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents the list of states the game can be in
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

    // Current turn
    public int turn = 0;

    // The game's state
    public GameState gameState = GameState.Player0Turn;
    public Level activeLevel;

    public List<EndTurnEffect> EndTurnEffects;

    // Events
    public event Action OnTurnChangeEvent;
    public event Action OnNPCTurnEvent;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("TurnController singleton assigned in error");
        else
            instance = this;

        EndTurnEffects = new List<EndTurnEffect>();
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
                turn++;
                break;
        }
        activeLevel.RefreshFOV();
        OnTurnChangeEvent?.Invoke();
    }

    public void EndTurn()
    {
        StartCoroutine(ProcessEndTurnEffects());
    }

    public IEnumerator ProcessEndTurnEffects()
    {
        foreach (EndTurnEffect effect in EndTurnEffects)
            yield return StartCoroutine(effect.DoEffect());
            
        EndTurnEffects.Clear();
        ChangeTurn();
    }

    // Check if player's turn to avoid unwieldy comparisons in other code
    public static bool IsPlayerTurn()
    {
        return instance.gameState == GameState.Player0Turn;
    }

    // Send event to all enemies allowing them to carry out their turn,
    // and then change back to players
    private void DoEnemyTurn()
    {
        OnNPCTurnEvent?.Invoke();
        EndTurn();
    }
}

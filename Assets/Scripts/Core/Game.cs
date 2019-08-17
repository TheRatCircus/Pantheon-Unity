// Main game loop handling
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

public class Game : MonoBehaviour
{
    // Singleton
    public static Game instance;

    // Basic prefabs
    public GameObject levelPrefab;

    // Current turn
    public int turn = 0;

    // The game's state
    public GameState gameState = GameState.Player0Turn;

    // Keep a global list of all players
    public Player player1;

    // Levels
    public List<Level> levels;
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

    // Start is called before the first frame update
    private void Start()
    {
        activeLevel.Initialize(true);
    }

    // Announce the end of a player turn
    public void EndTurn()
    {
        StartCoroutine(ProcessEndTurnEffects());
    }

    // Process all end-of-turn effects
    public IEnumerator ProcessEndTurnEffects()
    {
        foreach (EndTurnEffect effect in EndTurnEffects)
            yield return StartCoroutine(effect.DoEffect());

        EndTurnEffects.Clear();
        ChangeTurn();
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
    
    // Load a level into the scene
    public void LoadLevel(Level level)
    {
        Level lastLevel = activeLevel;
        lastLevel.gameObject.SetActive(false);
        activeLevel = level;
        level.gameObject.SetActive(true);
        EndTurn();
    }

    // Construct a new level
    public Level MakeNewLevel()
    {
        GameObject newLevelObj = Instantiate(levelPrefab);
        Level newLevel = newLevelObj.GetComponent<Level>();
        levels.Add(newLevel);
        return newLevel;
    }
}

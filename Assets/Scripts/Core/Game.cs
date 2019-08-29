// Main game loop handling
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Game : MonoBehaviour
{
    // Singleton
    public static Game instance;

    // Basic prefabs
    public GameObject levelPrefab;

    // Constants
    public const int TurnTime = 100; // One turn: waiting, picking things up
    public const int ActorsPerUpdate = 1000;

    List<Actor> queue;
    Actor currentActor;
    bool currentActorRemoved;

    int lockCount;

    // Keep a global list of all players
    public Player player1;

    // Levels
    public List<Level> levels;
    public Level activeLevel;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Game singleton assigned in error");
        else
            instance = this;

        queue = new List<Actor>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        AddActor(player1);
        activeLevel.Initialize(true);
    }

    private void Update()
    {
        for (int i = 0; i < queue.Count; i++)
            if (!Tick())
                break;
    }

    public void AddActor(Actor actor) => queue.Add(actor);
    public void RemoveActor(Actor actor)
    {
        queue.Remove(actor);

        if (currentActor == actor)
            currentActorRemoved = true;
    }

    public void Lock() => lockCount++;
    public void Unlock()
    {
        if (lockCount == 0)
            throw new Exception("Cannot unlock turn scheduler when not locked");

        lockCount--;
    }

    private bool Tick()
    {
        if (lockCount > 0)
            return false;

        if (queue.Count <= 0)
            throw new Exception("Turn queue should not be empty");

        Actor actor = queue[0];
        if (actor == null)
            return false;

        if (currentActorRemoved)
        {
            currentActorRemoved = false;
            return true;
        }

        while (actor.energy > 0)
        {
            currentActor = actor;
            int actionCost = actor.Act();
            currentActor = null;

            if (currentActorRemoved)
            {
                currentActorRemoved = false;
                return true;
            }

            // Handle asynchronous input by returning -1
            if (actionCost < 0)
                return false;

            actor.energy -= actionCost;

            // Action may have added a lock
            if (lockCount > 0)
                return false;
        }

        // Give the actor their speed value's worth of energy back
        actor.energy += actor.speed;
        Actor dequeued = queue[0];
        queue.RemoveAt(0);
        queue.Add(dequeued);

        activeLevel.RefreshFOV();

        return true;
    }

    // Load a level into the scene
    public void LoadLevel(Level level)
    {
        Level lastLevel = activeLevel;
        lastLevel.gameObject.SetActive(false);
        activeLevel = level;
        level.gameObject.SetActive(true);
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

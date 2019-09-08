// Game.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.WorldGen;

namespace Pantheon.Core
{
    /// <summary>
    /// Central game controller. Handles turn scheduling and holds other core game behaviour.
    /// </summary>
    public sealed class Game : MonoBehaviour
    {
        // Singleton
        public static Game instance;

        // Other components of GameController
        public Database database;
        public GameLog gameLog;
        public Transform grid;

        // Pseudo RNG
        public System.Random prng = new System.Random(131198);

        // Basic prefabs
        public GameObject levelPrefab;

        // Constants
        public const int TurnTime = 100; // One standard turn
        public const int ActorsPerUpdate = 1000;

        List<Actor> queue;
        Actor currentActor;
        bool currentActorRemoved;

        // Once 100 energy has been spent by the player,
        //  a turn is considered to have passed
        int turnProgress;
        int turns;

        int lockCount;

        // Keep a global list of all players
        public Player player1;

        // Levels
        public List<Level> levels;
        public Level activeLevel;

        // Events
        public event Action<int> OnTurnChangeEvent;
        public event Action<int> OnPlayerActionEvent;
        public event Action<Level> OnLevelChangeEvent;

        // Accessors
        public static Player GetPlayer(int i = 0) => instance.player1;
        public static System.Random PRNG() => instance.prng;

        /// <summary>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            if (instance != null)
                Debug.LogWarning("Game singleton assigned in error");
            else
                instance = this;

            queue = new List<Actor>();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            AddActor(player1);

            Level firstLevel = MakeNewLevel();
            LevelZones.GenerateValley(ref firstLevel, CardinalDirection.Centre);
            LoadLevel(firstLevel);

            player1.level = activeLevel;
            player1.transform.SetParent(activeLevel.transform);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < queue.Count; i++)
                if (!Tick())
                    break;
        }

        // Add and remove actors from the queue
        public void AddActor(Actor actor) => queue.Add(actor);
        public void RemoveActor(Actor actor)
        {
            queue.Remove(actor);

            if (currentActor == actor)
                currentActorRemoved = true;
        }

        // Temporarily lock and then unlock the scheduler recursively to allow
        // things to happen between ticks
        public void Lock() => lockCount++;
        public void Unlock()
        {
            if (lockCount == 0)
                throw new Exception("Cannot unlock turn scheduler when not locked");

            lockCount--;
        }

        // Iterate through each actor in the queue, and take its actions until its
        // energy is spent
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

                if (actionCost == 0)
                    Debug.LogWarning("An action with 0 energy cost was scheduled");

                // Handle asynchronous input by returning -1
                if (actionCost < 0)
                    return false;

                actor.energy -= actionCost;
                activeLevel.RefreshFOV();

                if (actor is Player)
                {
                    turnProgress += actionCost;
                    if (turnProgress >= TurnTime)
                    {
                        turns++;
                        turnProgress %= TurnTime;
                        OnTurnChangeEvent?.Invoke(turns);
                    }

                    // Signals a successful player action to HUD
                    OnPlayerActionEvent?.Invoke(actor.energy);
                }

                // Action may have added a lock
                if (lockCount > 0)
                    return false;
            }

            // Give the actor their speed value's worth of energy back
            actor.energy += actor.speed;

            // Update HUD again to reflect refill
            if (actor is Player)
                OnPlayerActionEvent?.Invoke(actor.energy);

            Actor dequeued = queue[0];
            queue.RemoveAt(0);
            queue.Add(dequeued);

            return true;
        }

        /// <summary>
        /// Load a level into the game scene, make it active in the hierarchy, and
        /// make it the active level.
        /// </summary>
        /// <param name="level">The level to be loaded.</param>
        public void LoadLevel(Level level)
        {
            Level lastLevel;
            if (activeLevel != null)
            {
                lastLevel = activeLevel;
                lastLevel.gameObject.SetActive(false);
            }

            activeLevel = level;
            level.gameObject.SetActive(true);
        }

        /// <summary>
        /// Move a player to a different level.
        /// </summary>
        public void MoveToLevel(Player player, Level level, Cell cell)
        {
            player.transform.SetParent(level.transform);
            player.level = level;
            Actor.MoveTo(player, cell);
            instance.LoadLevel(level);
            level.RefreshFOV();
            OnLevelChangeEvent?.Invoke(level);
        }

        /// <summary>
        /// Instantiates a new level from a prefab.
        /// </summary>
        /// <returns>The Level script component of the new level GameObject.</returns>
        public Level MakeNewLevel()
        {
            GameObject newLevelObj = Instantiate(levelPrefab, grid);
            Level newLevel = newLevelObj.GetComponent<Level>();
            levels.Add(newLevel);
            return newLevel;
        }

        public static void QuitGame() => SceneManager.LoadScene("Title");
    }
}

// Game.cs
// Jerome Martina
// Credit to Dan Korostelev

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
    /// Central game controller. Handles turn scheduling and holds other core 
    /// game behaviour.
    /// </summary>
    public sealed class Game : MonoBehaviour
    {
        // Singleton
        public static Game instance;

        // Other components of GameController
        [SerializeField] private Database database;
        [SerializeField] private GameLog gameLog;
        [SerializeField] private Transform grid = null;

        // Pseudo RNG
        public System.Random prng = new System.Random(131198);

        // Basic prefabs
        [SerializeField] private GameObject levelPrefab = null;

        // Constants
        public const int TurnTime = 100; // One standard turn

        private List<Actor> queue;
        private Actor currentActor;
        private bool currentActorRemoved;

        // Once 100 energy has been spent by the player,
        //  a turn is considered to have passed
        [ReadOnly] [SerializeField] private int turnProgress;
        [ReadOnly] [SerializeField] private int turns;

        [ReadOnly] [SerializeField] private int lockCount;

        // Keep a global list of all players
        public Player player1;

        // Levels
        public Dictionary<string, Level> levels = new Dictionary<string, Level>();
        public Level activeLevel;

        public bool IdolMode { get; set; }

        // Events
        public event Action OnTurnChangeEvent;
        public event Action<int> OnClockTickEvent;
        public event Action<int> OnPlayerActionEvent;
        public event Action<Level> OnLevelChangeEvent;
        public event Action<Actor> ActorDebugEvent;

        // Properties
        public Database Database { get => database; set => database = value; }
        public GameLog GameLog { get => gameLog; set => gameLog = value; }

        // Accessors
        public static Player GetPlayer(int i = 0) => instance.player1;
        public static System.Random PRNG() => instance.prng;

        /// <summary>
        /// Awake is called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            if (instance != null)
                UnityEngine.Debug.LogWarning("Game singleton assigned in error");
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

            player1.OnPlayerDeathEvent += Lock;
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

            while (actor.Energy > 0)
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
                    UnityEngine.Debug.LogWarning
                        ("An action with 0 energy cost was scheduled");

                ActorDebugEvent?.Invoke(actor);

                // Handle asynchronous input by returning -1
                if (actionCost < 0)
                    return false;

                actor.Energy -= actionCost;
                activeLevel.RefreshFOV();

                if (actor is Player)
                {
                    turnProgress += actionCost;
                    if (turnProgress >= TurnTime)
                    {
                        int turnsPassed = turnProgress / TurnTime;
                        turns += turnsPassed;
                        turnProgress %= TurnTime;

                        for (int i = 0; i < turnsPassed; i++)
                            OnTurnChangeEvent?.Invoke();
                        OnClockTickEvent?.Invoke(turns);
                    }

                    // Signals a successful player action to HUD
                    OnPlayerActionEvent?.Invoke(actor.Energy);
                }

                // Action may have added a lock
                if (lockCount > 0)
                    return false;
            }

            // Give the actor their speed value's worth of energy back
            actor.Energy += actor.Speed;

            // Update HUD again to reflect refill
            if (actor is Player)
                OnPlayerActionEvent?.Invoke(actor.Energy);

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
            return newLevel;
        }

        /// <summary>
        /// After level generation, pass it here to add it to the world map.
        /// </summary>
        public void RegisterLevel(Level level)
            => levels.Add(level.RefName, level);

        public static void QuitGame() => SceneManager.LoadScene("Title");
    }
}

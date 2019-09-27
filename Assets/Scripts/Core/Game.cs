// Game.cs
// Jerome Martina
// Credit to Dan Korostelev

using Pantheon.Actors;
using Pantheon.Utils;
using Pantheon.World;
using Pantheon.WorldGen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pantheon.Core
{
    /// <summary>
    /// Central game controller. Turn scheduling and other core game behaviour.
    /// </summary>
    public sealed class Game : MonoBehaviour
    {
        public const int TurnTime = 100; // One standard turn

        // Singleton
        public static Game instance;

        // Other components of GameController
        public System.Random prng = new System.Random();
        [SerializeField] private Database database;
        [SerializeField] private GameLog gameLog;
        [SerializeField] private Transform grid = null;
        public Pantheon Pantheon { get; private set; }

        // Basic prefabs
        [SerializeField] private GameObject levelPrefab = null;

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

        // Game world
        public Level activeLevel;
        public Dictionary<int, Layer> Layers { get; set; }
            = new Dictionary<int, Layer>();
        // All levels need a ref, and not all 
        // levels fit into the tangible world-space
        public Dictionary<string, Level> Levels { get; set; }
            = new Dictionary<string, Level>();

        public Dictionary<string, LevelBuilder> BuilderMap
            { get; set; }
            = new Dictionary<string, LevelBuilder>();

        // Factions
        public Faction Nature { get; set; }
        public Dictionary<Idol, Faction> Religions { get; set; }
            = new Dictionary<Idol, Faction>();

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
        public static System.Random PRNG => instance.prng;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Initialize singleton
            if (instance != null)
                UnityEngine.Debug.LogWarning("Game singleton assigned in error");
            else
                instance = this;

            GameObject introObj = GameObject.FindGameObjectWithTag("Intro");
            Intro intro = introObj.GetComponent<Intro>();

            player1.ActorName = intro.PlayerName;
            player1.Initialize();
            player1.AddItem(ItemFactory.NewWeapon(intro.StartingWeapon));

            introObj.GetComponentInChildren<Camera>().enabled = false;
            introObj.SetActive(false);

            queue = new List<Actor>();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            Pantheon = new Pantheon();
            InitializeFactions();

            foreach (Idol idol in Pantheon.Idols.Values)
                for (int i = 0; i < 3; i++) // i < number of levels per domain
                    BuilderMap.Add($"domain_{idol.RefName}_{i}",
                        new DomainBuilder(null, Vector2Int.zero));

            AddActor(player1);

            int overworldZ = 0;
            Layer overworld = new Layer(overworldZ);
            Layers.Add(overworldZ, overworld);
            Level firstLevel = overworld.RequestLevel(new Vector2Int(0, 0));
            
            LoadLevel(firstLevel);

            player1.level = activeLevel;
            player1.transform.SetParent(activeLevel.transform);

            player1.OnPlayerDeathEvent += Lock;
            UnityEngine.Debug.Log("Game initialization complete.");
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

        public void InitializeFactions()
        {
            Nature = new Faction("Nature", "nature", FactionType.Nature, null);
            foreach (Idol idol in Pantheon.Idols.Values)
            {
                string displayName = $"The Church of {idol.DisplayName}";
                string refName = $"religion{idol.DisplayName}";
                Faction religion = new Faction(displayName, refName,
                    FactionType.Religion, idol);
                Religions.Add(idol, religion);
                idol.Religion = religion;
            }
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
                throw new Exception
                    ("Cannot unlock turn scheduler when not locked");

            lockCount--;
        }

        // Iterate through each actor in the queue, and
        // take its actions until its energy is spent
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
                    activeLevel.RefreshFOV();
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
        /// Load a level into the game scene, make it active in the hierarchy,
        /// and make it the active level.
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
            if (!level.Visited)
                level.Visited = true;
        }

        /// <summary>
        /// Instantiates a new level from a prefab.
        /// </summary>
        /// <returns>The Level script component of the new level GameObject.
        /// </returns>
        public Level MakeNewLevel()
        {
            GameObject newLevelObj = Instantiate(levelPrefab, grid);
            Level newLevel = newLevelObj.GetComponent<Level>();
            return newLevel;
        }

        /// <summary>
        /// After gen of a level, pass it here to add it to the world map.
        /// </summary>
        public void RegisterLevel(Level level)
            => Levels.Add(level.RefName, level);

        public Level RequestLevel(string prevLevelRef, string levelRef)
        {
            if (!Levels.ContainsKey(levelRef))
            {
                if (!BuilderMap.TryGetValue(levelRef,
                    out LevelBuilder builder))
                    throw new ArgumentException("Bad ref given.");

                Level newLevel = instance.MakeNewLevel();
                builder.Generate(newLevel);

                if (Levels.Count > 0 && prevLevelRef != null)
                    ConnectLevels(newLevel, prevLevelRef);

                return newLevel;
            }
            else
            {
                Levels.TryGetValue(levelRef, out Level newLevel);
                return newLevel;
            }
        }

        public void ConnectLevels(Level newLevel, string prevRef)
        {
            UnityEngine.Debug.Log
                ($"Attempting to connect {newLevel.RefName} to {prevRef}...");

            if (!Levels.TryGetValue(prevRef, out Level prevLevel))
                throw new ArgumentException("Bad ref given for previous level.");

            if (newLevel.LateralConnections.Count > 0)
            {
                foreach (KeyValuePair<CardinalDirection, Connection> pair
                    in newLevel.LateralConnections)
                {
                    CardinalDirection dir = pair.Key;
                    Connection homeConn = pair.Value;

                    if (!prevLevel.LateralConnections.TryGetValue(Helpers.CardinalOpposite(dir),
                        out Connection otherConn))
                        throw new Exception("Other level has no valid opposite.");

                    homeConn.SetDestination(otherConn);
                }
            }

            if (newLevel.DownConnections.HasElements())
            {
                for (int i = 0; i < newLevel.DownConnections.Length; i++)
                {
                    if (newLevel.DownConnections[i] != null)
                    {
                        if (prevLevel.UpConnections[i] != null)
                            newLevel.DownConnections[i].SetDestination
                                (prevLevel.UpConnections[i]);
                        else
                            throw new Exception("Other has no compatible" +
                                "upwards connection.");
                    }
                }
            }
        }

        public static void QuitGame()
        {
            CharacterNames.ClearUsed();
            SceneManager.LoadScene("Title");
        }
    }
}

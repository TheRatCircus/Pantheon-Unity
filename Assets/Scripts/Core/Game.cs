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
    /// Central game (not application) controller.
    /// </summary>
    public sealed class Game : MonoBehaviour
    {
        public const int TurnTime = 100; // One standard turn

        // Singleton
        public static Game instance;

        // Other components of GameController
        private System.Random prng;
        private int seed;
        [SerializeField] private Database database;
        [SerializeField] private GameLog gameLog;
        [SerializeField] private GameObject grid = null;
        [SerializeField] private GameObject hud = null;
        [SerializeField] private GameObject worldGUI = null;
        [SerializeField] private GameObject eventSystem = null;
        // Don't serialize pantheon due to reliance on PRNG
        [NonSerialized] private Pantheon pantheon = null;

        // Basic prefabs
        [SerializeField] private GameObject levelPrefab = null;

        private List<Actor> queue = new List<Actor>();
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

        // Active save
        private Save save;

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
        public static Pantheon Pantheon
        {
            get => instance.pantheon;
            private set => instance.pantheon = value;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            UnityEngine.Debug.Log("Waking game controller...");
            // Initialize singleton
            if (instance != null)
                UnityEngine.Debug.LogWarning
                    ("Game singleton assigned in error");
            else
                instance = this;

            // Guarantee that everything else is disabled first
            foreach (GameObject go in SceneManager.GetSceneByName(
                        Scenes.Game).GetRootGameObjects())
                if (go != gameObject)
                    go.SetActive(false);
        }

        private System.Collections.IEnumerator LoadDebugScene()
        {
            AsyncOperation debugLoad = SceneManager.LoadSceneAsync
                (Scenes.Debug, LoadSceneMode.Additive);

            while (!debugLoad.isDone)
                yield return null;

            UnityEngine.Debug.Log("Debug scene loaded.");
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

        public void NewGame(string playerName, ItemDef startGear)
        {
            UnityEngine.Debug.Log("Starting a new game...");

            player1.OnPlayerDeathEvent += Lock;

            prng = new System.Random(UnityEngine.Random.Range(int.MinValue,
                int.MaxValue));

            Pantheon = new Pantheon();
            InitializeFactions();

            foreach (Idol idol in Pantheon.Idols.Values)
                for (int i = 0; i < 3; i++) // i < number of levels per sanctum
                    BuilderMap.Add($"sanctum_{idol.RefName}_{i}",
                        new SanctumBuilder(null, Vector2Int.zero));

            grid.SetActive(true);

            int overworldZ = 0;
            Layer overworld = new Layer(overworldZ);
            Layers.Add(overworldZ, overworld);
            Level firstLevel = overworld.RequestLevel(new Vector2Int(0, 0));

            LoadLevel(firstLevel);

            player1.gameObject.SetActive(true);

            player1.level = activeLevel;
            player1.transform.SetParent(activeLevel.transform);

            hud.SetActive(true);
            worldGUI.SetActive(true);
            eventSystem.SetActive(true);

            player1.ActorName = playerName;
            player1.Initialize();
            AddActor(player1);

            StartCoroutine(LoadDebugScene());
            player1.AddItem(new Item(startGear));

            //save = new Save();
            //WriteToSave();
            //SaveLoad.Save(save);
        }

        private void WriteToSave()
        {
            save.SaveName = player1.ActorName;
            save.Seed = seed;
            //save.Pantheon = Pantheon;
            //save.Queue = queue;
            //save.CurrentActor = currentActor;
            save.TurnProgress = turnProgress;
            save.Turns = turns;
            //save.Player = player1;
            //save.ActiveLevel = activeLevel;
            save.Layers = Layers;
            //save.Levels = Levels;
            save.BuilderMap = BuilderMap;
            save.Nature = Nature;
            save.Religions = Religions;
            save.IdolMode = IdolMode;
        }

        private void ReadFromSave()
        {
            seed = save.Seed;
            //Pantheon = save.Pantheon;
            //queue = save.Queue;
            //currentActor = save.CurrentActor;
            turnProgress = save.TurnProgress;
            turns = save.Turns;
            //player1 = save.Player;
            //activeLevel = save.ActiveLevel;
            Layers = save.Layers;
            //Levels = save.Levels;
            BuilderMap = save.BuilderMap;
            Nature = save.Nature;
            Religions = save.Religions;
            IdolMode = save.IdolMode;
        }

        public void SaveAndQuit()
        {
            //WriteToSave();
            QuitToTitle();
        }

        public static void LoadGame(Save save)
        {
            instance.save = save;
            instance.ReadFromSave();
            instance.prng = new System.Random(instance.seed);

            instance.hud.SetActive(true);
            instance.worldGUI.SetActive(true);
            instance.activeLevel.RefreshFOV();
            UnityEngine.Debug.Log
                ($"Successfully loaded save game {save.SaveName}.");
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
            GameObject newLevelObj = Instantiate(levelPrefab, grid.transform);
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
                throw new ArgumentException
                    ("Bad ref given for previous level.");

            if (newLevel.LateralConnections.Count > 0)
            {
                foreach (KeyValuePair<CardinalDirection, Connection> pair
                    in newLevel.LateralConnections)
                {
                    CardinalDirection dir = pair.Key;
                    Connection homeConn = pair.Value;

                    if (!prevLevel.LateralConnections.TryGetValue
                        (Helpers.CardinalOpposite(dir),
                        out Connection otherConn))
                        throw new Exception
                            ("Other level has no valid opposite.");

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

        public static void QuitToTitle()
        {
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

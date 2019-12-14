// GameController.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Gen;
using Pantheon.SaveLoad;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cursor = Pantheon.UI.Cursor;

namespace Pantheon.Core
{
    public enum GameState
    {
        Default,
        PlayerDead
    }

    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject levelPrefab = default;
        public GameObject LevelPrefab => levelPrefab;
        [SerializeField] private GameObject gameObjectPrefab = default;
        public GameObject GameObjectPrefab => gameObjectPrefab;
        [SerializeField] private Transform worldTransform = default;
        public Transform WorldTransform => worldTransform;

        [SerializeField] private Camera cam = default;
        [SerializeField] private Cursor cursor = default;
        [SerializeField] private HUD hud = default;
        public Cursor Cursor => cursor;
        [SerializeField] private GameLog log = default;
        public GameLog Log => log;
        public PlayerControl PlayerControl { get; private set; }

        public GameWorld World { get; private set; }
        public AssetLoader Loader { get; private set; }
        public LevelGenerator Generator { get; private set; }
        public TurnScheduler Scheduler { get; private set; }
        private SaveWriterReader saveSystem;

        public GameState State { get; private set; } = GameState.Default;
        public Entity Player
        {
            get => PlayerControl.PlayerEntity;
            set => PlayerControl.PlayerEntity = value;
        }

        public event System.Action<Level> LevelChangeEvent;

        private void OnEnable()
        {
            Loader = GetComponent<AssetLoader>();
            Scheduler = GetComponent<TurnScheduler>();
            SchedulerLocator.Provide(Scheduler);
            PlayerControl = GetComponent<PlayerControl>();
            LogLocator.Provide(Log);
        }

        public void InjectStaticDependencies()
        {
            AI.InjectController(this);
            Spawn.InjectController(this);
            GameWorld.InjectController(this);
            LevelGenerator.InjectController(this);
        }

        public void NewGame(string playerName)
        {
            InjectStaticDependencies();
            saveSystem = new SaveWriterReader(Loader);

            World = new GameWorld();
            Generator = new LevelGenerator();
            
            // Place the world centre
            World.NewLayer(0);
            World.Layers.TryGetValue(0, out Layer surface);
            Generator.GenerateWorldOrigin();
            Level level = surface.RequestLevel(Vector2Int.zero);
            
            // Spawn the player
            EntityTemplate template = Loader.LoadTemplate("Player");
            Entity player = Spawn.SpawnActor(template, level, level.RandomCell(true));
            player.DestroyedEvent += OnPlayerDeath;

            Player = player;

            hud.Initialize(Scheduler, Player, level, LevelChangeEvent);
            LoadLevel(level, true);
            MoveCameraTo(player.GameObjects[0].transform);
            cursor.Level = level;
        }

        public void LoadGame(string path)
        {
            InjectStaticDependencies();
            saveSystem = new SaveWriterReader(Loader);

            Save save = saveSystem.ReadSave(path);

            World = save.World;
            Generator = save.Generator;
            Player = save.Player;

            hud.Initialize(Scheduler, Player, World.ActiveLevel, LevelChangeEvent);
            LoadLevel(Player.Level, false);
            MoveCameraTo(Player.GameObjects[0].transform);
            cursor.Level = Player.Level;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveGame();
                QuitToTitle();
            }
        }

        /// <summary>
        /// Clear turn queue, set active level, create GameObject, draw level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="refreshFOV"></param>
        private void LoadLevel(Level level, bool refreshFOV)
        {
            Scheduler.Queue.Clear();

            World.ActiveLevel = level;

            level.AssignGameObject(Instantiate(levelPrefab, worldTransform).transform);

            if (refreshFOV)
                FOV.RefreshFOV(level, Player.Cell.Position, false);

            foreach (Cell c in level.Map)
            {
                if (c.Actor != null)
                {
                    Spawn.AssignGameObject(c.Actor);
                    Scheduler.AddActor(c.Actor.GetComponent<Actor>());
                }
                level.DrawTile(c);
            }

            LevelChangeEvent?.Invoke(level);
        }

        private void UnloadLevel(Level level)
        {
            Destroy(level.Transform.gameObject);
            World.ActiveLevel = null;
        }

        private void MoveCameraTo(Transform transform)
        {
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0, 0, -1);
        }

        public void AllowInputToCharacter(bool allow)
        {
            PlayerControl.SendingInput = allow;
        }

        private void SaveGame()
        {
            Save save = new Save(Player.Name, World, Generator, Player);
            saveSystem.WriteSave(save);
        }

        private void OnPlayerDeath()
        {
            State = GameState.PlayerDead;
        }

        public static void QuitToTitle()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

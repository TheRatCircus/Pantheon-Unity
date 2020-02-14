// GameController.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.SaveLoad;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using System;
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
        private static GameController _inst;

        [SerializeField] private GameObject levelPrefab = default;
        [SerializeField] private GameObject gameObjectPrefab = default;
        [SerializeField] private Transform worldTransform = default;
        public GameObject LevelPrefab => levelPrefab;
        public GameObject GameObjectPrefab => gameObjectPrefab;
        public Transform WorldTransform => worldTransform;

        [SerializeField] private Camera cam = default;
        [SerializeField] private Cursor cursor = default;
        [SerializeField] private HUD hud = default;
        [SerializeField] private GameLog log = default;
        public Cursor Cursor => cursor;
        public GameLog Log => log;
        public Player Player { get; private set; }

        [NonSerialized] private GameWorld world;
        public GameWorld World
        {
            get => world;
            private set => world = value;
        }
        public TurnScheduler Scheduler { get; private set; }
        private SaveWriterReader saveSystem;
        public AudioManager Audio { get; private set; }

        public GameState State { get; private set; } = GameState.Default;
        public Entity PC
        {
            get => Player.Entity;
            set => Player.Entity = value;
        }
        public Level ActiveLevel { get; set; }

        public event Action<Level> LevelChangeEvent;

        private void Awake() => _inst = this;

        private void OnEnable()
        {
            Scheduler = GetComponent<TurnScheduler>();
            Locator.Scheduler = Scheduler;
            Player = GetComponent<Player>();
            Locator.Player = Player;
            Locator.Log = Log;
            Audio = GetComponent<AudioManager>();
            Locator.Audio = Audio;
        }

        public void NewGame(string playerName)
        {
            saveSystem = new SaveWriterReader();

            World = new GameWorld() { Plan = Assets.WorldPlan };
            Layer subterrane = World.NewLayer(-2);
            Level level = subterrane.RequestLevel(Vector2Int.zero);

            // Spawn the player
            EntityTemplate template = Assets.GetTemplate("Player");
            Vector2Int spawnCell = Floodfill.QueueFillForCell(
                level, level.RandomCorner(),
                (Vector2Int c) => false, 
                (Vector2Int d) => !level.Blocked(d.x, d.y));
            Entity player = Spawn.SpawnActor(template, level, spawnCell);
            player.DestroyedEvent += OnPlayerDeath;

            PC = player;
            Player.UpdateHotbar();

            hud.Initialize(Scheduler, PC, level, LevelChangeEvent);
            LoadLevel(level, true);
            MoveCameraTo(player.GameObjects[0].transform);
            cursor.Level = level;
        }

        public void LoadGame(string path)
        {
            saveSystem = new SaveWriterReader();

            Save save = saveSystem.ReadSave(path);

            World = save.World;
            PC = save.Player;
            PC.DestroyedEvent += OnPlayerDeath;

            hud.Initialize(Scheduler, PC, ActiveLevel, LevelChangeEvent);
            LoadLevel(PC.Level, false);
            MoveCameraTo(PC.GameObjects[0].transform);
            cursor.Level = PC.Level;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveGame();
                QuitToTitle();
            }
        }

        public static void Travel(Connection connection)
        {
            Level prev = _inst.PC.Level;
            Level destLevel = _inst.World.RequestLevel(connection);
            Vector2Int cell = connection.Partner.Position;
            _inst.PC.Move(destLevel, cell);
            _inst.LoadLevel(destLevel, true);
            _inst.UnloadLevel(prev);
        }

        /// <summary>
        /// Clear turn queue, set active level, create GameObject, draw level.
        /// </summary>
        public void LoadLevel(Level level, bool refreshFOV)
        {
            ActiveLevel = level;
            Scheduler.Queue.Clear();
            level.AssignGameObject(Instantiate(levelPrefab, worldTransform).transform);

            if (refreshFOV)
                FOV.RefreshFOV(level, PC.Cell, false);

            foreach (Vector2Int c in level.Map)
            {
                Entity e = level.ActorAt(c);
                if (e != null)
                {
                    AssignGameObject(e);
                    Scheduler.AddActor(e.GetComponent<Actor>());
                }
                level.DrawTile(c);
            }

            LevelChangeEvent?.Invoke(level);
        }

        public void UnloadLevel(Level level)
        {
            Destroy(level.Transform.gameObject);
        }

        public void AssignGameObject(Entity entity)
        {
            if (entity.GameObjects == null)
                entity.GameObjects = new GameObject[1];

            if (entity.GameObjects[0] != null)
            {
                entity.GameObjects[0].transform.SetParent(entity.Level.EntitiesTransform);
                return;
            }

            GameObject entityObj = Instantiate(
                GameObjectPrefab,
                entity.Cell.ToVector3(),
                new Quaternion(),
                entity.Level.EntitiesTransform);

            entityObj.name = entity.Name;
            EntityWrapper wrapper = entityObj.GetComponent<EntityWrapper>();
            wrapper.Entity = entity;
            SpriteRenderer sr = entityObj.GetComponent<SpriteRenderer>();
            sr.sprite = entity.Flyweight.Sprite;

            if (!entity.Visible)
                sr.enabled = false;

            entity.GameObjects[0] = entityObj;
        }

        private void MoveCameraTo(Transform transform)
        {
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0, 0, -1);
        }

        public void AllowInputToCharacter(bool allow)
        {
            Locator.Player.Mode = allow ? InputMode.Default : InputMode.None;
        }

        public void SaveGame()
        {
            Save save = new Save(PC.Name, World, PC);
            saveSystem.WriteSave(save);
        }

        private void OnPlayerDeath()
        {
            Player.enabled = false;
            State = GameState.PlayerDead;
        }

        public static void QuitToTitle()
        {
            FOV.ResetPrevious();
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

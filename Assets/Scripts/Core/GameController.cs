// GameController.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Content;
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
        public Player Player { get; private set; }

        public GameWorld World { get; private set; }
        public LevelGenerator Generator { get; private set; }
        public TurnScheduler Scheduler { get; private set; }
        private SaveWriterReader saveSystem;

        public GameState State { get; private set; } = GameState.Default;
        public Entity PC
        {
            get => Player.Entity;
            set => Player.Entity = value;
        }

        public event System.Action<Level> LevelChangeEvent;

        private void OnEnable()
        {
            Scheduler = GetComponent<TurnScheduler>();
            Locator.Scheduler = Scheduler;
            Player = GetComponent<Player>();
            Locator.Player = Player;
            Locator.Log = Log;
        }

        public void NewGame(string playerName)
        {
            saveSystem = new SaveWriterReader();

            Generator = new LevelGenerator();
            World = new GameWorld(Generator);

            World.NewLayer(-2);
            World.Layers.TryGetValue(-2, out Layer surface);
            Generator.PlaceBuilders();
            Level level = surface.RequestLevel(Vector2Int.zero);
            
            // Spawn the player
            EntityTemplate template = Assets.Templates["Player"];
            Entity player = Spawn.SpawnActor(template, level, level.RandomCell(true));
            player.DestroyedEvent += OnPlayerDeath;

            PC = player;

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
            Generator = save.Generator;
            PC = save.Player;
            PC.DestroyedEvent += OnPlayerDeath;

            hud.Initialize(Scheduler, PC, World.ActiveLevel, LevelChangeEvent);
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

        /// <summary>
        /// Clear turn queue, set active level, create GameObject, draw level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="refreshFOV"></param>
        public void LoadLevel(Level level, bool refreshFOV)
        {
            World.ActiveLevel = level;
            Scheduler.Queue.Clear();
            level.AssignGameObject(Instantiate(levelPrefab, worldTransform).transform);

            if (refreshFOV)
                FOV.RefreshFOV(level, PC.Cell, false);

            foreach (Cell c in level.Map)
            {
                if (c.Actor != null)
                {
                    AssignGameObject(c.Actor);
                    Scheduler.AddActor(c.Actor.GetComponent<Actor>());
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
                entity.Cell.Position.ToVector3(),
                new Quaternion(),
                entity.Level.EntitiesTransform);

            entityObj.name = entity.Name;
            EntityWrapper wrapper = entityObj.GetComponent<EntityWrapper>();
            wrapper.Entity = entity;
            SpriteRenderer sr = entityObj.GetComponent<SpriteRenderer>();
            sr.sprite = entity.Flyweight.Sprite;

            if (!entity.Cell.Visible)
                sr.enabled = false;

            entity.GameObjects = new GameObject[1];
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
            Save save = new Save(PC.Name, World, Generator, PC);
            saveSystem.WriteSave(save);
        }

        private void OnPlayerDeath()
        {
            Player.enabled = false;
            State = GameState.PlayerDead;
        }

        public static void QuitToTitle()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

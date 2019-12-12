// GameController.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS;
using Pantheon.Gen;
using Pantheon.SaveLoad;
using Pantheon.UI;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pantheon.Core
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject levelPrefab = default;
        public GameObject LevelPrefab => levelPrefab;
        [SerializeField] private GameObject gameObjectPrefab = default;
        public GameObject GameObjectPrefab => gameObjectPrefab;
        [SerializeField] private Transform worldTransform = default;
        public Transform WorldTransform => worldTransform;

        [SerializeField] private Camera cam = default;
        [SerializeField] private UI.Cursor cursor = default;
        public UI.Cursor Cursor => cursor;
        [SerializeField] private GameLog log = default;
        public GameLog Log => log;
        public PlayerControl PlayerControl { get; private set; }

        public GameWorld World { get; private set; }
        public AssetLoader Loader { get; private set; }
        public LevelGenerator Generator { get; private set; }
        public TurnScheduler Scheduler { get; private set; }
        private SaveWriterReader saveSystem;

        // ECS
        public EntityManager EntityManager { get; private set; }
        public SystemManager SysManager { get; private set; }

        private void OnEnable()
        {
            Loader = GetComponent<AssetLoader>();
            Scheduler = GetComponent<TurnScheduler>();
            PlayerControl = GetComponent<PlayerControl>();
        }

        public void InjectStaticDependencies()
        {
            GameWorld.InjectController(this);
            LevelGenerator.InjectController(this);
        }

        public void NewGame(string playerName)
        {
            InjectStaticDependencies();

            EntityManager = new EntityManager();
            EntityManager.NewActorEvent += OnNewActor;
            PlayerControl.Initialize(EntityManager);

            SysManager = new SystemManager(EntityManager, log, Scheduler);
            saveSystem = new SaveWriterReader(Loader);
            Scheduler.ActionDoneEvent += SysManager.Update;

            World = new GameWorld();
            Generator = new LevelGenerator();
            
            // Place the world centre
            World.NewLayer(0);
            World.Layers.TryGetValue(0, out Layer surface);
            Generator.GenerateWorldOrigin();
            Level level = surface.RequestLevel(Vector2Int.zero);
            cursor.Level = level;

            // Spawn the player
            EntityTemplate template = Loader.LoadTemplate("Player");
            Entity player = EntityManager.NewEntity(
                template, level, level.RandomCell(true));
            EntityManager.Player = player;

            LoadLevel(level, true);
            MoveCameraTo(EntityManager.Player.GameObjects[0].transform);
        }

        public void LoadGame(string path)
        {
            InjectStaticDependencies();
            saveSystem = new SaveWriterReader(Loader);

            Save save = saveSystem.ReadSave(path);

            World = save.World;
            Generator = save.Generator;
            EntityManager.Player = save.Player;

            LoadLevel(EntityManager.LevelOfPlayer(), false);
            MoveCameraTo(EntityManager.Player.GameObjects[0].transform);
            cursor.Level = EntityManager.LevelOfPlayer();
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
                FOV.RefreshFOV(level,
                    EntityManager.CellOfPlayer().Position, false);

            foreach (Cell c in level.Map)
            {
                if (c.Actor != null)
                {
                    AssignGameObject(c.Actor);
                    Scheduler.AddActor(EntityManager.Actors[c.Actor.GUID]);
                }
                level.DrawTile(c);
            }
        }

        private void UnloadLevel(Level level)
        {
            Destroy(level.Transform.gameObject);
            World.ActiveLevel = null;
        }

        public void AssignGameObject(Entity entity)
        {
            GameObject entityObj = Instantiate(GameObjectPrefab,
                EntityManager.CellOf(entity).Position.ToVector3(),
                new Quaternion(),
                EntityManager.LevelOf(entity).EntitiesTransform);

            entityObj.name = entity.Name;
            EntityWrapper wrapper = entityObj.GetComponent<EntityWrapper>();
            wrapper.Entity = entity;
            SpriteRenderer sr = entityObj.GetComponent<SpriteRenderer>();
            sr.sprite = entity.Flyweight.Sprite;

            if (!EntityManager.CellOf(entity).Visible)
                sr.enabled = false;

            entity.GameObjects = new GameObject[1];
            entity.GameObjects[0] = entityObj;
        }

        /// <summary>
        /// Schedule a new actor for turns if its level is active.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="level"></param>
        /// <param name="visible">If the new actor's cell is visible.</param>
        private void OnNewActor(Actor actor, Level level, bool visible)
        {
            if (level == World.ActiveLevel)
                Scheduler.AddActor(actor);
            //if (visible)
            //    PlayerControl.VisibleActors.Add()
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
            Save save = new Save(EntityManager.Player.Name, World, Generator,
                EntityManager.Player);
            saveSystem.WriteSave(save);
        }

        public static void QuitToTitle()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

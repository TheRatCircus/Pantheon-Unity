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

namespace Pantheon.Core
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject levelPrefab = default;
        public GameObject LevelPrefab => levelPrefab;
        [SerializeField] private GameObject gameObjectPrefab = default;
        [SerializeField] private Transform worldTransform = default;
        public Transform WorldTransform => worldTransform;

        [SerializeField] private Camera cam = default;
        [SerializeField] private UI.Cursor cursor = default;
        public UI.Cursor Cursor => cursor;
        [SerializeField] private GameLog log = default;
        public GameLog Log => log;
        private PlayerInput playerInput = default;

        public GameWorld World { get; private set; }
        public AssetLoader Loader { get; private set; }
        public LevelGenerator Generator { get; private set; }
        public TurnScheduler Scheduler { get; private set; }
        private SaveWriterReader saveSystem;

        public Entity Player { get; set; }

        private void OnEnable()
        {
            Loader = GetComponent<AssetLoader>();
            Scheduler = GetComponent<TurnScheduler>();
            playerInput = GetComponent<PlayerInput>();
        }

        public void InjectStaticDependencies()
        {
            AI.InjectController(this);
            Spawn.Init(Scheduler, gameObjectPrefab);
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
            cursor.Level = level;

            // Spawn the player
            EntityTemplate template = Loader.LoadTemplate("Player");
            Entity player = Spawn.SpawnActor(template, level, level.RandomCell(true));
            playerInput.SetPlayerEntity(player);
            
            Player = player;

            LoadLevel(level, true);
            MoveCameraTo(player.GameObjects[0].transform);
        }

        public void LoadGame(string path)
        {
            InjectStaticDependencies();
            saveSystem = new SaveWriterReader(Loader);

            Save save = saveSystem.ReadSave(path);

            World = save.World;
            Generator = save.Generator;
            Player = save.Player;

            playerInput.SetPlayerEntity(Player);
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
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="refreshFOV"></param>
        private void LoadLevel(Level level, bool refreshFOV)
        {
            Scheduler.Queue.Clear();

            World.ActiveLevel = level;

            level.AssignGameObject(Instantiate(levelPrefab, worldTransform).transform);

            if (refreshFOV)
                FOV.RefreshFOV(level, Player.Cell.Position);

            foreach (Cell c in level.Map.Values)
            {
                level.DrawTile(c);
                if (c.Actor != null)
                {
                    Spawn.AssignGameObject(c.Actor);
                    Scheduler.AddActor(c.Actor.GetComponent<Actor>());
                }
            }
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
            playerInput.SendingInput = allow;
        }

        private void SaveGame()
        {
            Save save = new Save(Player.Name, World, Generator, Player);
            saveSystem.WriteSave(save);
        }

        public static void QuitToTitle()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

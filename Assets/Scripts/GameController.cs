// GameController.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.ECS.Components;
using Pantheon.ECS.Systems;
using Pantheon.ECS.Templates;
using Pantheon.Gen;
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
        [SerializeField] private Template playerTemplate = default;

        [SerializeField] private GameObject worldGameObject = default;
        public GameObject WorldGameObject => worldGameObject;
        [SerializeField] private Camera cam = default;
        [SerializeField] private UI.Cursor cursor = default;
        public UI.Cursor Cursor => cursor;
        [SerializeField] private SystemManager systems = default;

        public GameWorld World { get; private set; } = default;
        public EntityFactory EntityFactory { get; private set; } = default;
        public EntityManager Manager { get; private set; }
        public AssetLoader Loader { get; private set; }

        public static void NewGame(string playerName)
        {
            // TODO: TEST CODE
            // Find the GameController
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();

            // Start all subsystems
            ctrl.Loader = new AssetLoader();
            ctrl.Manager = new EntityManager();
            ctrl.systems.Initialize(ctrl.Manager);
            ctrl.systems.enabled = true;
            ctrl.EntityFactory = new EntityFactory(
                ctrl, ctrl.systems.GetSystem<PositionSystem>());

            // Place the world centre
            GameObject worldObj = GameObject.FindGameObjectWithTag("GameWorld");
            ctrl.World = new GameWorld(new LevelGenerator(ctrl));
            ctrl.World.Layers.TryGetValue(0, out Layer surface);
            surface.Levels.TryGetValue(Vector2Int.zero, out Level level);

            // Spawn the player
            Entity playerEntity = ctrl.EntityFactory.NewEntityAt(
                ctrl.playerTemplate, level, level.RandomCell(true), false);
            playerEntity.GetComponent<Actor>().ActorControl = ActorControl.Player;
            Player player = playerEntity.GetComponent<Player>();
            PlayerSystem sys =
                ctrl.GetComponentInChildren<SystemManager>().
                GetSystem<PlayerSystem>();
            sys.InputMessageEvent += player.SendInput;
            
            ctrl.LoadLevel(level);
            ctrl.MoveCameraTo(playerEntity);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveGame();
                QuitToTitle();
            }
        }

        public GameController Get()
        {
            return this;
        }

        private void LoadLevel(Level level)
        {
            World.ActiveLevel = level;
            level.AssetRequestEvent += Loader.GetAsset;
            foreach (Cell cell in level.Map.Values)
            {
                level.VisualizeTile(cell);
                if (cell.Blocked &&
                    cell.Blocker.TryGetComponent(out UnityGameObject go))
                {
                    GameObject gameObj = Instantiate(gameObjectPrefab,
                        level.LevelObj.transform);
                    gameObj.name = cell.Blocker.Name;
                    gameObj.GetComponent<SpriteRenderer>().sprite = cell.Blocker.Flyweight.Sprite;
                    gameObj.transform.position = Helpers.V2IToV3(cell.Position);
                    go.GameObject = gameObj;
                }
            }
            cursor.Level = level;
        }

        private void UnloadLevel(Level level)
        {

        }

        // Move the camera to a new transform when the Player 
        // component changes entity, or when the game starts up
        private void MoveCameraTo(Entity entity)
        {
            cam.transform.SetParent(
                entity.GetComponent<UnityGameObject>().GameObject.transform);
            cam.transform.localPosition = new Vector3(0, 0, -1);
        }

        public void AllowInputToCharacter(bool allow)
        {
            systems.GetSystem<PlayerSystem>().InputToCharacter = allow;
        }

        public void SaveGame()
        {
            Save save = new Save(Manager.Player.Name, World, Manager);
            SaveLoad.Save(save);
        }

        public static void LoadGame(string path)
        {
            Save save = SaveLoad.Load(path);

            // Find the GameController
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();

            ctrl.World = save.World;
            ctrl.Manager = save.Manager;
        }

        public static void QuitToTitle()
        {
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

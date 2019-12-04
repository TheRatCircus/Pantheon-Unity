// GameController.cs
// Jerome Martina

using Pantheon.Gen;
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

        [SerializeField] private GameObject worldGameObject = default;
        public GameObject WorldGameObject => worldGameObject;
        [SerializeField] private Camera cam = default;
        [SerializeField] private UI.Cursor cursor = default;
        public UI.Cursor Cursor => cursor;
        [SerializeField] private GameLog log = default;

        public GameWorld World { get; private set; } = default;
        public AssetLoader Loader { get; private set; }
        public LevelGenerator LevelGenerator { get; private set; }

        public static void NewGame(string playerName)
        {
            // Find the GameController
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();

            // Start all subsystems
            ctrl.Loader = new AssetLoader();
            ctrl.LevelGenerator = new LevelGenerator(ctrl);

            // Place the world centre
            ctrl.World = new GameWorld(ctrl.LevelGenerator);
            ctrl.World.Layers.TryGetValue(0, out Layer surface);
            surface.Levels.TryGetValue(Vector2Int.zero, out Level level);

            // Spawn the player

            ctrl.LoadLevel(level, true);
        }

        public static void LoadGame(Save save)
        {
            // Find the GameController
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();

            // Start all subsystems
            ctrl.Loader = new AssetLoader();
            ctrl.LevelGenerator = new LevelGenerator(ctrl);
            ctrl.World = new GameWorld(save, ctrl.LevelGenerator);

            ctrl.LoadLevel(ctrl.World.ActiveLevel, false);
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

        /// <summary>
        /// V
        /// </summary>
        /// <param name="level"></param>
        /// <param name="refreshFOV"></param>
        private void LoadLevel(Level level, bool refreshFOV)
        {
            
        }

        private void UnloadLevel(Level level)
        {
            World.ActiveLevel = null;
            level.AssetRequestEvent -= Loader.Load<Object>;
        }

        private void MoveCameraTo(Transform transform)
        {
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0, 0, -1);
        }

        public void AllowInputToCharacter(bool allow)
        {
            
        }

        public void SaveGame()
        {
            Save save = new Save();
            SaveLoad.Save(save);
        }

        public static void LoadGame(string path)
        {
            Save save = SaveLoad.Load(path);

            // Find the GameController
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();

            ctrl.World = save.World;
        }

        public static void QuitToTitle()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

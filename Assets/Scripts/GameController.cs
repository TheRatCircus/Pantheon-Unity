// GameController.cs
// Jerome Martina

using Pantheon.Components;
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
        [SerializeField] private GameObject gameObjectPrefab = default;
        [SerializeField] private Camera cam = default;
        [SerializeField] private UI.Cursor cursor = default;
        public UI.Cursor Cursor => cursor;
        [SerializeField] private GameLog log = default;
        private PlayerInput playerInput = default;

        [SerializeField] GameWorld world = default;
        public GameWorld World => world;
        private AssetLoader loader = default;
        [SerializeField] private LevelGenerator gen = default;
        private TurnScheduler scheduler;

        public Entity Player { get; set; }

        private void OnEnable()
        {
            loader = GetComponent<AssetLoader>();
            scheduler = GetComponent<TurnScheduler>();
            playerInput = GetComponent<PlayerInput>();
        }

        public void NewGame(string playerName)
        {
            AI.Init(Player, log);
            Spawn.Init(scheduler, gameObjectPrefab);

            // Place the world centre
            world.NewLayer(0);
            world.Layers.TryGetValue(0, out Layer surface);
            gen.GenerateWorldOrigin();
            Level level = surface.RequestLevel(Vector2Int.zero);
            cursor.Level = level;

            // Spawn the player
            EntityTemplate template = loader.LoadTemplate("Player");
            Entity player = Spawn.SpawnActor(template, level, level.RandomCell(true));
            playerInput.SetPlayerEntity(player);
            MoveCameraTo(player.GameObjects[0].transform);
            Player = player;

            FOV.RefreshFOV(level, player.Cell.Position);
            LoadLevel(level, true);
        }

        public void LoadGame(Save save)
        {

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
            level.gameObject.SetActive(true);
            level.Draw();
        }

        private void UnloadLevel(Level level)
        {
            World.ActiveLevel = null;
            level.AssetRequestEvent -= loader.Load<Object>;
        }

        private void MoveCameraTo(Transform transform)
        {
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0, 0, -1);
        }

        public void AllowInputToCharacter(bool allow)
        {
            
        }

        void SaveGame()
        {

        }

        public static void QuitToTitle()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

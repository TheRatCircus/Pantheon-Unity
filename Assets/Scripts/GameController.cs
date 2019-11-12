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
        [SerializeField] private Template playerTemplate = default;

        [SerializeField] private Camera cam = default;
        [SerializeField] private UI.Cursor cursor = default;
        public UI.Cursor Cursor => cursor;
        [SerializeField] private SystemManager systems = default;

        public GameWorld World { get; private set; } = default;
        public EntityFactory EntityFactory { get; private set; } = default;
        public EntityManager Manager { get; private set; }

        public static void NewGame(string playerName)
        {
            // TODO: TEST CODE
            // Find the GameController
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();

            // Start all subsystems
            ctrl.Manager = new EntityManager();
            ctrl.EntityFactory = new EntityFactory(ctrl.Manager);
            ctrl.systems.Initialize(ctrl.Manager);
            ctrl.systems.enabled = true;

            // Place the world centre
            GameObject worldObj = GameObject.FindGameObjectWithTag("GameWorld");
            ctrl.World = new GameWorld(new LevelGenerator(
                ctrl.EntityFactory, ctrl.levelPrefab, worldObj));

            // Spawn the player
            ctrl.World.Layers.TryGetValue(0, out Layer surface);
            surface.Levels.TryGetValue(Vector2Int.zero, out Level level);
            ctrl.cursor.Level = level;
            level.TryGetCell(10, 10, out Cell c);

            Entity playerEntity = ctrl.EntityFactory.NewEntityAt(
                ctrl.playerTemplate, level, c);
            playerEntity.GetComponent<Actor>().ActorControl = ActorControl.Player;
            Player player = playerEntity.GetComponent<Player>();
            PlayerSystem sys =
                ctrl.GetComponentInChildren<SystemManager>().PlayerSystem;
            sys.InputMessageEvent += player.SendInput;
            ctrl.MoveCameraTo(playerEntity);

            ctrl.systems.UpdatePerTurnSystems();
            level.Render();
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
            systems.PlayerSystem.InputToCharacter = allow;
        }

        public static void QuitToTitle()
        {
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

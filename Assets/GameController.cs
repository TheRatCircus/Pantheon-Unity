// GameController.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS.Systems;
using Pantheon.ECS.Templates;
using Pantheon.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pantheon.ECS
{
    public sealed class GameController : MonoBehaviour
    {
        [SerializeField] private Template playerTemplate = default;

        [SerializeField] private Camera cam = default;
        [SerializeField] private SystemManager systems = default;
        [SerializeField] private World world = default;

        public EntityFactory EntityFactory { get; private set; } = default;
        public EntityManager Manager { get; private set; }

        public static void NewGame(string playerName)
        {
            // TODO: TEST CODE
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();

            ctrl.Manager = new EntityManager();
            ctrl.EntityFactory = new EntityFactory(ctrl.Manager);
            ctrl.systems.Initialize(ctrl.Manager);
            ctrl.systems.enabled = true;

            ctrl.world.gameObject.SetActive(true);
            ctrl.world.Level.Map.TryGetValue(new Vector2Int(32, 32), out Cell c);

            Entity playerEntity = ctrl.EntityFactory.NewEntityAt(
                ctrl.playerTemplate, ctrl.world.Level, c);
            playerEntity.GetComponent<Actor>().ActorControl = ActorControl.Player;
            Player player = playerEntity.GetComponent<Player>();
            PlayerSystem sys =
                ctrl.GetComponentInChildren<SystemManager>().PlayerSystem;
            sys.InputMessageEvent += player.SendInput;

            ctrl.MoveCameraTo(playerEntity);
            ctrl.systems.UpdatePerTurnSystems();
        }

        // Move the camera to a new transform when 
        // the Player component changes entity
        private void MoveCameraTo(Entity entity)
        {
            cam.transform.SetParent(
                entity.GetComponent<UnityGameObject>().GameObject.transform);
            cam.transform.localPosition = new Vector3(0, 0, -1);
        }

        public static void QuitToTitle()
        {
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

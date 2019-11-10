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

        [SerializeField] private World world = default;
        public EntityFactory EntityFactory { get; private set; } = default;

        public static void NewGame(string playerName)
        {
            // TODO: TEST CODE
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();
            ctrl.world.gameObject.SetActive(true);
            ctrl.world.Level.Map.TryGetValue(new Vector2Int(32, 32), out Cell c);

            Entity playerEntity = EntityFactory.NewEntity(ctrl.playerTemplate);
            Player player = playerEntity.GetComponent<Player>();

            GameObject controller = GameObject.FindGameObjectWithTag(
                "GameController");
            PlayerSystem sys =
                controller.GetComponentInChildren<PlayerSystem>();
            sys.InputMessageEvent += player.SendInput;

            Camera cam = Camera.main;
            cam.transform.SetParent(
                playerEntity.
                GetComponent<UnityGameObject>().GameObject.transform);

            c.AddEntity(playerEntity);
        }

        public static void QuitToTitle()
        {
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

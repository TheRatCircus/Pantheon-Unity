// GameController.cs
// Jerome Martina

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

        public static void NewGame(string playerName)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("GameController");
            GameController ctrl = obj.GetComponent<GameController>();
            ctrl.world.gameObject.SetActive(true);
            ctrl.world.Level.Map.TryGetValue(new Vector2Int(32, 32), out Cell c);
            c.AddEntity(EntityFactory.NewEntity(ctrl.playerTemplate));
        }

        public static void QuitToTitle()
        {
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}

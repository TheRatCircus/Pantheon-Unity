// DebugInfo.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS;
using Pantheon.ECS.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pantheon.Debug
{
    public sealed class DebugInfo : MonoBehaviour
    {
        private GameController controller;

        [SerializeField] private Text activeActor;

        private void Awake()
        {
            Scene game = SceneManager.GetSceneByName(Utils.Scenes.Game);
            foreach (GameObject go in game.GetRootGameObjects())
            {
                if (go.tag == "GameController")
                {
                    controller = go.GetComponent<GameController>();
                    break;
                }
            }
            controller.Systems.ActorSystem.ActorDebugEvent += UpdateActiveActor;
        }

        private void UpdateActiveActor(Actor actor)
        {
            Entity e = controller.Manager.GetEntity(actor.GUID);
            activeActor.text = $"{e.Name} ({actor.Energy})";
        }
    }
}

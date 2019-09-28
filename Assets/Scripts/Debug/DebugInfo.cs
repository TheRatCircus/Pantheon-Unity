// DebugInfo.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.Debug
{
    public class DebugInfo : MonoBehaviour
    {
        [SerializeField] private Text activeActor = null;
        [SerializeField] private Text inputState = null;
        [SerializeField] private Text worldPos = null;
        [SerializeField] private Text levelPos = null;

        private void Start()
        {
            Game.instance.ActorDebugEvent += UpdateActiveActor;
        }

        void UpdateActiveActor(Actor actor)
            => activeActor.text
            = $"Active actor: {actor.ActorName} ({actor.Energy})";

        void UpdateInputState(InputState inputState)
            => this.inputState.text = $"Input state: {inputState}";

        void UpdateWorldPos(Vector3Int worldPos)
            => this.worldPos.text = $"Position of level in world: {worldPos}";

        void UpdateLevelPos(Vector2Int levelPos)
            => this.levelPos.text = $"Cell position in level: {levelPos}";
    }
}

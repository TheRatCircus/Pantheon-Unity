// DebugInfo.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.Debug
{
    public sealed class DebugInfo : MonoBehaviour
    {
        [SerializeField] private Text activeActor = default;

        public void Initialize(GameController ctrl)
        {
            ctrl.Scheduler.ActorDebugEvent += UpdateActiveActor;
        }

        private void UpdateActiveActor(Actor actor)
        {
            activeActor.text = actor.ToString();
        }
    }
}

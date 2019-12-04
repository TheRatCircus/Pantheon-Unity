// DebugInfo.cs
// Jerome Martina

using Pantheon.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.Debug
{
    public sealed class DebugInfo : MonoBehaviour
    {
        private GameController controller;

        [SerializeField] private Text activeActor = default;

        private void Awake()
        {

        }
    }
}

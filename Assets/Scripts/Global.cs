// Global.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    public sealed class Global : MonoBehaviour
    {
        public static Global Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}

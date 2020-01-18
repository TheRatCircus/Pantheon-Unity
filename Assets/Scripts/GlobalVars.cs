// GlobalVars.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    public sealed class GlobalVars : MonoBehaviour
    {
        public static GlobalVars Inst { get; private set; }

        private void Awake() => Inst = this;
    }
}

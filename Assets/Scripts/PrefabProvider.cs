// PrefabProvider.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    public sealed class PrefabProvider : MonoBehaviour
    {
        public static PrefabProvider Inst { get; private set; }

        [SerializeField] private GameObject tossFXPrefab = default;

        public static GameObject TossFXPrefab { get => Inst.tossFXPrefab; }

        private void Awake()
        {
            Inst = this;
        }
    }
}


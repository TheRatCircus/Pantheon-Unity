// NPCWrapper.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// NPC prefab with an enumeration and spawning configurations.
    /// </summary>
    [CreateAssetMenu(fileName = "New NPCWrapper",
        menuName = "BaseData/NPCWrapper")]
    public sealed class NPCWrapper : ScriptableObject
    {
        [SerializeField] private NPCType type = NPCType.None;
        [SerializeField] private GameObject prefab = null;

        [SerializeField] private bool packSpawn = false;
        [SerializeField] private int minPackSize = -1;
        [SerializeField] private int maxPackSize = -1;

        public NPCType Type { get => type; }
        public GameObject Prefab { get => prefab; }
        public bool PackSpawn { get => packSpawn; }
        public int MinPackSize { get => minPackSize; }
        public int MaxPackSize { get => maxPackSize; }
    }
}

// World.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS
{
    public sealed class World : MonoBehaviour
    {
        [SerializeField] private Level level = default;
        public Level Level { get => level; }
    }
}

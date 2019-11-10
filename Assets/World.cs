// World.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS
{
    public sealed class World : MonoBehaviour
    {
        [SerializeField] private Level level = default;
        public Level Level { get => level; }

        public static void MoveEntity(Position position, Level level, Cell cell)
        {
            position.Level = level;
            position.Cell = cell;
        }
    }
}

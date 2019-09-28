// Node.cs
// Credit to Sebastian Lague

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// A pathfinding node corresponding to a cell.
    /// </summary>
    public sealed class Node
    {
        // Properties
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;

        public Vector2Int Position { get; }
        public bool Blocked { get; set; }
        public Node Parent { get; set; }

        public Node(bool blocked, Vector2Int position)
        {
            Blocked = blocked;
            Position = position;
        }
    }
}

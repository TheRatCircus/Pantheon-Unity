// Node.cs
// Courtesy of Sebastian Lague

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// A pathfinding node corresponding to a cell.
    /// </summary>
    [System.Serializable]
    public sealed class Node : IHeapItem<Node>
    {
        public Vector2Int Cell { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public Node Parent { get; set; }

        public int HeapIndex { get; set; }

        public Node(Vector2Int cell) => Cell = cell;

        public override string ToString() => $"PF Node: {Cell}";

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);

            if (compare == 0)
                compare = HCost.CompareTo(other.HCost);

            return -compare;
        }
    }
}

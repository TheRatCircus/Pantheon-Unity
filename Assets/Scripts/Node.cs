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
        public Cell Cell { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public Node Parent { get; set; }
        public Vector2Int Position => Cell.Position;

        public int HeapIndex { get; set; }

        public Node(Cell cell) => Cell = cell;

        // TODO: No need for string interpolation here
        public override string ToString() => $"{Position}";

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);

            if (compare == 0)
                compare = HCost.CompareTo(other.HCost);

            return -compare;
        }
    }
}

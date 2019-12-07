// Node.cs
// Courtesy of Sebastian Lague

using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// A pathfinding node corresponding to a cell.
    /// </summary>
    [System.Serializable]
    public sealed class Node
    {
        public Cell Cell { get; set; }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public Node Parent { get; set; }
        public Vector2Int Position => Cell.Position;

        public Node(Cell cell) => Cell = cell;

        public override string ToString() => $"{Position}";
    }
}

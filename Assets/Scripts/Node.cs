// Pathfinding node corresponding to a cell
using UnityEngine;

public class Node
{
    // Properties
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost => GCost + HCost;

    public Vector2Int Position { get; }
    public bool Blocked { get; set; }
    public Node Parent { get; set; }

    // Constructor
    public Node(bool blocked, Vector2Int position)
    {
        Blocked = blocked;
        Position = position;
    }
}

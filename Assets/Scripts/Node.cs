// Pathfinding node corresponding to a cell
using UnityEngine;

public class Node
{
    // Previous node from which costs are derived
    public Node parent;

    public bool blocked;
    public Vector2Int position;

    // Pathfinding costs
    public int GCost;
    public int HCost;

    // Properties
    public int FCost => GCost + HCost;

    // Constructor
    public Node(bool blocked, Vector2Int position)
    {
        this.blocked = blocked;
        this.position = position;
    }
}

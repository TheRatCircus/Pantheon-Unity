// Base class for actors
using UnityEngine;

public class Actor : MonoBehaviour
{
    // Locational
    public Level currentLevel;
    protected Cell cell;

    // Actor's personal attributes
    protected int health;
    protected int maxHealth;
    protected int meleeDamage;

    // Properties
    public Cell _Cell { get => cell; }
    public Vector2Int Position { get => cell.Position; }

    // Move this actor to a new cell by reference
    public void MoveToCell(Cell cell)
    {
        // Save previous cell temporarily
        Cell prevCell = this.cell;
        Vector2 newPosition = new Vector2(cell.Position.x, cell.Position.y);
        transform.position = newPosition;
        // Reassign new cell
        cell._Actor = this;
        this.cell = cell;
        // Empty previous cell
        if (prevCell != null)
            prevCell._Actor = null;
    }

    // Move this actor to a new cell by position
    public void MoveToCell(Vector2Int pos)
    {
        // Save previous cell temporarily
        Cell prevCell = cell;
        Vector2 newPosition = new Vector2(pos.x, pos.y);
        transform.position = newPosition;
        // Reassign new cell
        Cell newCell = currentLevel.GetCell(pos);
        cell = newCell;
        newCell._Actor = this;
        // Empty previous cell
        if (prevCell != null)
            prevCell._Actor = null;
    }

    // Calc distance to another cell
    public int DistanceTo(Cell other)
    {
        return (int)Vector2Int.Distance(other.Position, cell.Position);
    }

    // Calc distance to another actor
    public int DistanceTo(Actor other)
    {
        return (int)Vector2Int.Distance(other._Cell.Position, _Cell.Position);
    }
}

// Base class for actors
using UnityEngine;
using System;

public class Actor : MonoBehaviour
{
    // Locational
    public Level level;
    protected Cell cell;

    // Actor's personal attributes
    protected string actorName;
    protected int health;
    protected int maxHealth;

    // Melee attributes
    protected int minDamage;
    protected int maxDamage;

    // Defense attributes
    protected int armour;
    protected int evasion;

    public GameObject corpsePrefab;

    // Properties
    public Cell _Cell { get => cell; }
    public Vector2Int Position { get => cell.Position; }
    public string ActorName { get => actorName; set => actorName = value; }
    public int Health
    {
        get => health;
        set
        {
            // TODO: Infinitely negative lower bound?
            health = Mathf.Clamp(value, -255, maxHealth);
            if (health <= 0)
                OnDeath();
            OnHealthChangeEvent?.Invoke();
        }
    }

    // Events
    public event Action OnHealthChangeEvent;

    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        health = maxHealth;
    }

    // Attempt to move to another cell by Vector
    protected virtual void TryMove(Vector2Int move)
    {
        Cell destinationCell;
        Vector2Int destinationPos =
            new Vector2Int((int)transform.position.x + move.x, (int)transform.position.y + move.y);
        destinationCell = level.Cells[destinationPos.x, destinationPos.y];
        if (destinationCell != null && destinationCell.IsWalkable())
            MoveToCell(destinationCell);
        else if (destinationCell._Actor != null)
            Attack(destinationCell);
    }

    // Attempt to move to another given Cell
    protected virtual void TryMove(Cell destinationCell)
    {
        if (destinationCell != null && destinationCell.IsWalkable())
            MoveToCell(destinationCell);
        else if (destinationCell._Actor != null)
            Attack(destinationCell);
    }

    // Move this actor to a new cell by reference
    public void MoveToCell(Cell cell)
    {
        // Save previous cell temporarily
        Cell prevCell = this.cell;
        transform.position = Helpers.GridToVector3(cell.Position);
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
        transform.position = Helpers.GridToVector3(pos);
        // Reassign new cell
        Cell newCell = level.GetCell(pos);
        cell = newCell;
        newCell._Actor = this;
        // Empty previous cell
        if (prevCell != null)
            prevCell._Actor = null;
    }

    // Make a melee attack on another cell
    public void Attack(Cell targetCell)
    {
        if (targetCell._Actor != null)
            TryToHit(targetCell._Actor);
    }

    // Try to hit a target actor
    public void TryToHit(Actor target)
    {
        if (UnityEngine.Random.Range(0, 101) > 60)
            MeleeHit(target);
        else
            GameLog.Send($"{(this is Player ? "You miss the goblin." : "The goblin misses you.")}", MessageColour.White);
    }

    // Deal damage to an actor with a successful melee hit
    public void MeleeHit(Actor target)
    {
        int damageDealt = UnityEngine.Random.Range(minDamage, maxDamage + 1);
        target.Health -= damageDealt;
        GameLog.Send($"{(this is Player ? "You hit the goblin." : "The goblin hits you.")}", MessageColour.White);
    }

    // Handle this actor's death
    protected virtual void OnDeath()
    {
        level.actors.Remove(this);
        Destroy(gameObject);
        GameLog.Send($"You kill the {actorName}!", MessageColour.White);
        MakeEntity.instance.MakeCorpseAt(corpsePrefab, cell);
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

// Base class for actors
using System;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected List<Item> inventory;

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

    public CorpseType corpse;

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
            OnHealthChangeEvent?.Invoke(health, maxHealth);
        }
    }
    public int MaxHealth { get => maxHealth; }
    public List<Item> Inventory { get => inventory; }

    // Events
    public event Action<int, int> OnHealthChangeEvent;
    public event Action<Cell> OnMoveEvent;

    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        health = maxHealth;
    }

    // Attempt to move to another given Cell
    protected virtual void TryMove(Cell destinationCell)
    {
        if (destinationCell != null && destinationCell.IsWalkable())
            MoveToCell(destinationCell);
        else if (destinationCell._actor != null)
            Attack(destinationCell);
    }

    // Move this actor to a new cell
    public void MoveToCell(Cell cell)
    {
        // Save previous cell temporarily
        Cell prevCell = this.cell;
        transform.position = Helpers.V2IToV3(cell.Position);
        // Reassign new cell
        cell._actor = this;
        this.cell = cell;
        // Empty previous cell
        if (prevCell != null)
            prevCell._actor = null;
        OnMoveEvent?.Invoke(cell);
    }

    // Make a melee attack on another cell
    public void Attack(Cell targetCell)
    {
        if (targetCell._actor != null)
            TryToHit(targetCell._actor);
    }

    // Try to hit a target actor
    public void TryToHit(Actor target)
    {
        if (UnityEngine.Random.Range(0, 101) > 60)
            MeleeHit(target);
        else if (target.health > 0)
            GameLog.Send($"{(this is Player ? "You miss the goblin." : "The goblin misses you.")}", MessageColour.White);
    }

    // Deal damage to an actor with a successful melee hit
    public void MeleeHit(Actor target)
    {
        int damageDealt = UnityEngine.Random.Range(minDamage, maxDamage + 1);
        GameLog.Send($"{(this is Player ? "You hit the goblin." : "The goblin hits you.")}", MessageColour.White);
        target.Health -= damageDealt;
    }

    // Attempt to pick up an item off the current cell
    protected virtual void TryPickup()
    {
    }

    // Pick up an item
    protected virtual void PickupItem(Item item)
    {
        inventory.Add(item);
    }

    // Handle this actor's death
    protected virtual void OnDeath()
    {
        level.actors.Remove(this);
        Destroy(gameObject);
        GameLog.Send($"You kill the {actorName}!", MessageColour.White);
        cell.Items.Add(new Item(Database.GetCorpse(corpse)));
    }
}

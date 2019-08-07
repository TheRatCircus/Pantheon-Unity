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
    public bool NameIsProper; // False if name should start with "The/the"

    protected int health;
    protected int maxHealth;

    // Melee attributes
    protected int minDamage;
    protected int maxDamage;

    // Defense attributes
    public int armour;
    public int evasion;

    // Per-actor-type data
    public CorpseType corpse;

    // Properties
    public Cell _cell { get => cell; }
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
    protected virtual void Awake() => health = maxHealth;

    // Attempt to move to another given Cell
    protected virtual void TryMove(Cell destinationCell)
    {
        if (destinationCell != null && destinationCell.IsWalkable())
            MoveToCell(destinationCell);
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
    protected virtual void Attack(Cell targetCell)
    {
        if (level.AdjacentTo(cell, targetCell))
            if (targetCell._actor != null)
                TryToHit(targetCell._actor);
            else
                GameLog.Send($"{GameLog.GetSubject(this, true)} {(this is Player ? "swing" :  "swings")} at nothing.", MessageColour.Grey);
        else
            Debug.LogWarning($"{actorName} attempted to make a melee attack on a non-adjacent cell.");
    }

    // Try to hit a target actor
    protected virtual void TryToHit(Actor target)
    {
        if (UnityEngine.Random.Range(0, 101) > 60)
            MeleeHit(target);
        else if (target.Health > 0) // Can't miss a dead target
            GameLog.Send($"{GameLog.GetSubject(this, true)} {(this is Player ? "miss" : "misses")} {GameLog.GetSubject(target, false)}.", MessageColour.Grey);
    }

    // Deal damage to an actor with a successful melee hit
    protected virtual void MeleeHit(Actor target)
    {
        int damageDealt = UnityEngine.Random.Range(minDamage, maxDamage + 1);
        GameLog.Send($"{GameLog.GetSubject(this, true)} {(this is Player ? "hit" : "hits")} {GameLog.GetSubject(target, false)}.", MessageColour.White);
        target.Health -= damageDealt;
    }

    // Attempt to pick up an item off the current cell
    protected virtual void TryPickup() { }

    // Pick up an item
    protected virtual void PickupItem(Item item) => inventory.Add(item);

    // Check if another actor is hostile to this
    public bool IsHostileToMe(Actor other)
    {
        if (this is Player) // Everything else is hostile to player (for now)
            return true;
        else // This is an enemy
            return other is Player; // If other is Player, it's hostile
    }

    // Handle this actor's death
    protected virtual void OnDeath()
    {
        level.actors.Remove(this);
        cell._actor = null;
        Destroy(gameObject);
        GameLog.Send($"You kill {GameLog.GetSubject(this, false)}!", MessageColour.White);
        cell.Items.Add(new Item(Database.GetCorpse(corpse)));
    }

    // ToString override
    public override string ToString() => actorName;
}

// Base class for actors

using System;
using System.Collections.Generic;
using UnityEngine;
using Pantheon.Utils;

public class Actor : MonoBehaviour
{
    protected List<Item> inventory;

    // Locational
    public Level level;
    [SerializeField] protected Cell cell;

    // Actor's personal attributes
    public string actorName;
    public bool NameIsProper; // False if name should start with "The/the"

    // Attributes
    protected int health;
    public int MaxHealth;

    public int speed; // Energy per turn
    public int energy; // Energy remaining

    public int moveSpeed; // Energy needed to walk one cell
    public int armour;
    public int evasion;

    public int minDamage;
    public int maxDamage;
    public int accuracy; // % chance out of 100
    public int attackTime;

    // Per-actor-type data
    public CorpseType corpse;

    public Pantheon.Actions.BaseAction nextAction;

    // Properties
    public Cell Cell { get => cell; set => cell = value; }
    public Vector2Int Position { get => cell.Position; }
    public int Health { get => health; }
    public List<Item> Inventory { get => inventory; }

    // Events
    public event Action<int, int> OnHealthChangeEvent;
    public event Action<Cell> OnMoveEvent;

    // Event invokers
    public void RaiseOnMoveEvent(Cell cell) => OnMoveEvent?.Invoke(cell);

    // Arbitrarily move an actor to a cell
    public static void MoveTo(Actor actor, Cell cell)
    {
        Cell previous = null;
        if (actor.cell != null)
            previous = actor.cell;

        if (!cell.IsWalkableTerrain())
        {
            Debug.LogException(new Exception("MoveTo destination is not walkable"));
            return;
        }

        if (cell._actor != null)
        {
            Debug.LogException(new Exception("MoveTo destination has an actor in it"));
            return;
        }

        actor.RaiseOnMoveEvent(cell);
        actor.transform.position = Helpers.V2IToV3(cell.Position);
        cell._actor = actor;
        actor.Cell = cell;

        // Empty previous cell if one exists
        if (previous != null)
            previous._actor = null;

        if (actor is Player)
            GameLog.LogCellItems(cell);
    }

    // Awake is called when the script instance is being loaded
    protected virtual void Awake() => health = MaxHealth;

    // Called by scheduler to carry out and process this actor's action
    public virtual int Act()
    { Debug.LogWarning("Attempted call of base Act()"); return -1; }

    // Take a damaging hit from something
    public void TakeHit(Hit hit) => TakeDamage(hit.Damage);

    // Receive damage
    public void TakeDamage(int damage)
    {
        // TODO: Infinitely negative lower bound?
        health = Mathf.Clamp(health - damage, -255, MaxHealth);
        if (health <= 0)
            OnDeath();
        OnHealthChangeEvent?.Invoke(health, MaxHealth);
    }

    // Recover health
    public void Heal(int healing)
    {
        health = Mathf.Clamp(health + healing, -255, MaxHealth);
        OnHealthChangeEvent?.Invoke(health, MaxHealth);
    }

    // Remove an item from this actor's inventory
    public virtual void RemoveItem(Item item)
    {
        inventory.Remove(item);
        item.Owner = null;
    }

    // Check if another actor is hostile to this
    public bool HostileToMe(Actor other)
    {
        if (this is Player) // Everything else is hostile to player (for now)
            return true;
        else // This is an enemy
            return other is Player; // If other is Player, it's hostile
    }

    // Handle this actor's death
    protected virtual void OnDeath()
    {
        Game.instance.RemoveActor(this);
        cell._actor = null;
        Destroy(gameObject);
        GameLog.Send($"You kill {GameLog.GetSubject(this, false)}!", MessageColour.White);
        cell.Items.Add(new Item(Database.GetCorpse(corpse)));
    }

    // ToString override
    public override string ToString() => actorName;
}

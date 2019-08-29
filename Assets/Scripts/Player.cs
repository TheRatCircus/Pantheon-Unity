// Player controller
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private PlayerInput input;

    protected int inventorySize = 40;
    public int InventorySize { get => inventorySize; }
    // FOV radius not in cells but the floating-point distance between player
    // and point in grid
    private int fovRadius = 15;

    private List<Cell> visibleCells;
    private List<Cell> movePath;

    // Properties
    public int FOVRadius { get => fovRadius; }
    public PlayerInput _input { get => input; }
    public List<Cell> MovePath { get => movePath; set => movePath = value; }

    // Events
    public event Action OnInventoryChangeEvent;
    public event Action OnInventoryToggleEvent;
    public event Action<Actor> OnAdvancedAttackEvent;

    // Event invokers for PlayerInput
    public void RaiseInventoryToggleEvent() => OnInventoryToggleEvent?.Invoke();

    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        inventory = new List<Item>(inventorySize);
        visibleCells = new List<Cell>();
        movePath = new List<Cell>();
    }

    // Start is called before the first frame update
    private void Start() => input = GetComponent<PlayerInput>();

    public override int Act()
    {
        if (nextAction != null)
        {
            Pantheon.Actions.BaseAction ret = nextAction;
            nextAction = null;
            return ret.DoAction();
        }
        else return input.Act();
    }

    // Attempt to move along a given path
    public int MoveAlongPath()
    {
        if (movePath.Count <= 0)
            return -1;

        foreach (Cell c in visibleCells)
            if (c._actor is Enemy)
            {
                GameLog.Send($"An enemy is nearby!", MessageColour.Red);
                movePath.Clear();
                return -1;
            }

        int ret = PlayerTryMove(movePath[0]);
        movePath.RemoveAt(0);
        return ret;
    }

    // Update the list of cells visible to this player
    public void UpdateVisibleCells(List<Cell> refreshed)
    {
        if (visibleCells != null)
            visibleCells.Clear();

        foreach (Cell c in refreshed)
            if (c.Visible) visibleCells.Add(c);
    }

    // Attempt to move to another given Cell
    private int PlayerTryMove(Cell cell)
    {
        if (cell != null && cell.IsWalkable())
        {
            PlayerMove(cell);
            return moveSpeed;
        }
        else if (cell._actor != null)
        {
            Attack(cell);
            return attackSpeed;
        }
        else return -1;
    }

    // Attempt to move to another cell by delta Vector
    public int PlayerTryMove(Vector2Int pos)
    {
        Cell cell = level.GetCell(this.cell.Position + pos);
        if (cell != null && cell.IsWalkable())
        {
            PlayerMove(cell);
            return moveSpeed;
        }
        else if (cell._actor != null)
        {
            Attack(cell);
            return attackSpeed;
        }
        else return -1;
    }

    // Actually make the move to another cell
    private void PlayerMove(Cell destinationCell)
    {
        MoveToCell(destinationCell);
        PrintTileContents();
    }

    // Try to make a melee attack on a target cell
    protected override void Attack(Cell targetCell)
    {
        if (level.AdjacentTo(cell, targetCell))
            if (targetCell._actor != null)
                TryToHit(targetCell._actor);
            else
                GameLog.Send($"{GameLog.GetSubject(this, true)} swing at nothing.", MessageColour.Grey);
        else
            Debug.LogWarning($"The player attempted to make a melee attack on a non-adjacent cell.");
    }

    // Try to hit a target actor
    protected override void TryToHit(Actor target)
    {
        if (UnityEngine.Random.Range(0, 101) > 40)
            MeleeHit(target);
        else if (target.Health > 0) // Can't miss a dead target
            GameLog.Send($"You miss {GameLog.GetSubject(target, false)}.", MessageColour.Grey);
    }

    // Deal damage to an actor with a successful melee hit
    protected override void MeleeHit(Actor target)
    {
        int damageDealt = UnityEngine.Random.Range(minDamage, maxDamage + 1);
        GameLog.Send($"You hit {GameLog.GetSubject(target, false)}.", MessageColour.White);
        target.Health -= damageDealt;
    }

    // Send the items in a cell to the game log
    private void PrintTileContents()
    {
        if (cell.Items.Count > 0)
        {
            string msg = $"You see here";
            int i = 0;
            for (; i < cell.Items.Count; i++)
                msg += $" a {cell.Items[i].DisplayName};";
            GameLog.Send(msg, MessageColour.Grey);
        }
    }

    #region Items

    // Attempt to pick up an item off the current cell
    new public void TryPickup()
    {
        if (cell.Items.Count > 0)
            PickupItem(cell.Items[0]);
        else
            GameLog.Send("There is nothing here to pick up.", MessageColour.Grey);
    }

    // Pick up an item
    protected override void PickupItem(Item item)
    {
        GameLog.Send($"You pick up a {item.DisplayName}.", MessageColour.White);
        cell.Items.Remove(item);
        inventory.Add(item);
        item.Owner = this;
        OnInventoryChangeEvent?.Invoke();
    }

    // Try to use an item
    public void TryUseItem(Item item)
    {
        if (item.Usable)
            item.Use(this);
        else
            GameLog.Send("This item cannot be used.", MessageColour.Grey);
    }

    // Remove item, but include inventory change event
    public override void RemoveItem(Item item)
    {
        base.RemoveItem(item);
        OnInventoryChangeEvent?.Invoke();
    }

    // Drop an item
    public void DropItem(Item item)
    {
        RemoveItem(item);
        GameLog.Send($"You drop the {item.DisplayName}", MessageColour.Grey);
    }

    #endregion

    #region AdvancedAttack

    public void StartAdvancedAttack()
    {
        input.PointTargetConfirmEvent += ConfirmAdvancedAttack;
        input.TargetCancelEvent += CancelAdvancedAttack;
    }

    public void ConfirmAdvancedAttack(Cell target)
    {
        if (target._actor == null)
            Attack(target);
        else if (target._actor == this)
            GameLog.Send("Why would you want to do that?", MessageColour.Teal);
        else
            OnAdvancedAttackEvent?.Invoke(target._actor);
        input.PointTargetConfirmEvent -= ConfirmAdvancedAttack;
        input.TargetCancelEvent -= CancelAdvancedAttack;
    }

    public void CancelAdvancedAttack()
    {
        input.PointTargetConfirmEvent -= ConfirmAdvancedAttack;
        input.TargetCancelEvent -= CancelAdvancedAttack;
    }

    #endregion

    // Handle the player's death
    protected override void OnDeath()
    {
        cell._actor = null;
        level.actors.Remove(this);
        GameLog.Send("You perish...", MessageColour.Purple);
    }
}

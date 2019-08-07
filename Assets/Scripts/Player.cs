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

    // Properties
    public int FOVRadius { get => fovRadius; }

    // Events
    public event Action OnInventoryChangeEvent;
    public event Action OnInventoryToggleEvent;
    public event Action<Actor> OnAdvancedAttackEvent;
    // Event invokers for PlayerInput
    public void RaiseInventoryToggleEvent() => OnInventoryToggleEvent?.Invoke();

    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        actorName = "Player";
        maxHealth = 10;
        minDamage = 2;
        maxDamage = 4;
        base.Awake();
        inventory = new List<Item>(inventorySize);
    }

    // Start is called before the first frame update
    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    // Attempt to move along a given path
    public void MoveAlongPath(List<Cell> path)
    {
        foreach (Cell cell in path)
        {
            bool nearbyEnemy = false;
            foreach (Actor actor in level.actors)
                if (actor is Enemy && actor._cell.Visible)
                    nearbyEnemy = true;
            if (nearbyEnemy)
            {
                GameLog.Send($"An enemy is nearby!", MessageColour.Red);
                break;
            }
            PlayerTryMove(cell);
        }
    }

    // Attempt to move to another given Cell
    private void PlayerTryMove(Cell cell)
    {
        if (cell != null && cell.IsWalkable())
            PlayerMove(cell);
        else if (cell._actor != null)
            Attack(cell);
    }

    // Attempt to move to another cell by delta Vector
    public void PlayerTryMove(Vector2Int pos)
    {
        Cell cell = level.GetCell(this.cell.Position + pos);
        if (cell != null && cell.IsWalkable())
            PlayerMove(cell);
        else if (cell._actor != null)
            Attack(cell);
    }

    // Actually make the move to another cell
    private void PlayerMove(Cell destinationCell)
    {
        MoveToCell(destinationCell);
        PrintTileContents();
        TurnController.instance.ChangeTurn();
    }

    // Wait a single turn
    public void WaitOneTurn()
    {
        TurnController.instance.ChangeTurn();
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
        if (UnityEngine.Random.Range(0, 101) > 80)
            MeleeHit(target);
        else if (target.Health > 0) // Can't miss a dead target
            GameLog.Send($"You miss {GameLog.GetSubject(target, false)}.", MessageColour.Grey);
        TurnController.instance.ChangeTurn();
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
        GameLog.Send($"You pick up a {item.DisplayName}", MessageColour.White);
        cell.Items.Remove(item);
        inventory.Add(item);
        item.Owner = this;
        OnInventoryChangeEvent?.Invoke();
        TurnController.instance.ChangeTurn();
    }

    // Try to use an item
    public void TryUseItem(Item item)
    {
        if (item.Usable)
            UseItem(item);
        else
            GameLog.Send("This item cannot be used.", MessageColour.Grey);
    }

    // Use an item
    private void UseItem(Item item)
    {
        inventory.Remove(item);
        item.OnUse();
        OnInventoryChangeEvent?.Invoke();
        TurnController.instance.ChangeTurn();
    }

    // Drop an item
    public void DropItem(Item item)
    {
        inventory.Remove(item);
        cell.Items.Add(item);
        OnInventoryChangeEvent?.Invoke();
        GameLog.Send($"You drop the {item.DisplayName}", MessageColour.Grey);
        TurnController.instance.ChangeTurn();
    }

    #endregion

    #region AdvancedAttack

    public void StartAdvancedAttack()
    {
        input.OnTargetConfirmEvent += ConfirmAdvancedAttack;
        input.OnTargetCancelEvent += CancelAdvancedAttack;
    }

    public void ConfirmAdvancedAttack(Cell target)
    {
        if (target._actor == null)
            Attack(target);
        else if (target._actor == this)
            GameLog.Send("Why would you want to do that?", MessageColour.Teal);
        else
            OnAdvancedAttackEvent?.Invoke(target._actor);
        input.OnTargetConfirmEvent -= ConfirmAdvancedAttack;
        input.OnTargetCancelEvent -= CancelAdvancedAttack;
    }

    public void CancelAdvancedAttack()
    {
        input.OnTargetConfirmEvent -= ConfirmAdvancedAttack;
        input.OnTargetCancelEvent -= CancelAdvancedAttack;
    }

    #endregion

    // Handle the player's death
    protected override void OnDeath()
    {
        cell._actor = null;
        level.actors.Remove(this);
        TurnController.instance.gameState = GameState.PlayersDead;
        GameLog.Send("You perish...", MessageColour.Purple);
    }
}

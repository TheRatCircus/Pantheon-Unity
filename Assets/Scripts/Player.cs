// Player controller
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    protected int inventorySize = 40;
    public int InventorySize { get => inventorySize; }
    private int fovRadius = 7;

    // Properties
    public int FOVRadius { get => fovRadius; }

    // Events
    public event System.Action OnInventoryChangeEvent;
    public event System.Action OnInventoryToggleEvent;

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

    // Update is called once per frame
    private void Update()
    {
        CatchInput();
    }

    // Receive input from the player on the player's turn
    private void CatchInput()
    {
        if (TurnController.instance.gameState == GameState.Player0Turn)
        {
            if (Input.GetButtonDown("Up"))
                PlayerTryMove(new Vector2Int(0, 1));
            else if (Input.GetButtonDown("Down"))
                PlayerTryMove(new Vector2Int(0, -1));
            else if (Input.GetButtonDown("Left"))
                PlayerTryMove(new Vector2Int(-1, 0));
            else if (Input.GetButtonDown("Right"))
                PlayerTryMove(new Vector2Int(1, 0));
            else if (Input.GetButtonDown("Up Left"))
                PlayerTryMove(new Vector2Int(-1, 1));
            else if (Input.GetButtonDown("Up Right"))
                PlayerTryMove(new Vector2Int(1, 1));
            else if (Input.GetButtonDown("Down Left"))
                PlayerTryMove(new Vector2Int(-1, -1));
            else if (Input.GetButtonDown("Down Right"))
                PlayerTryMove(new Vector2Int(1, -1));
            else if (Input.GetButtonDown("Wait"))
                TurnController.instance.ChangeTurn();
            else if (Input.GetButtonDown("Inventory"))
                OnInventoryToggleEvent?.Invoke();
            else if (Input.GetButtonDown("Pickup"))
                TryPickup();
        }
    }

    // Attempt to move along a given path
    public void MoveAlongPath(List<Cell> path)
    {
        foreach (Cell cell in path)
        {
            bool nearbyEnemy = false;
            foreach (Actor actor in level.actors)
                if (actor is Enemy && actor._Cell.Visible)
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
    private void PlayerTryMove(Vector2Int pos)
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
        level.RefreshFOV();
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

    // Attempt to pick up an item off the current cell
    protected override void TryPickup()
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
        level.RefreshFOV();
        GameLog.Send($"You drop the {item.DisplayName}", MessageColour.Grey);
        TurnController.instance.ChangeTurn();
    }

    // Handle the player's death
    protected override void OnDeath()
    {
        TurnController.instance.gameState = GameState.PlayersDead;
        GameLog.Send("You perish...", MessageColour.Purple);
    }
}

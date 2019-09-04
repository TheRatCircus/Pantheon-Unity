// Player controller
using System;
using System.Collections.Generic;
using Pantheon.Actions;

public class Player : Actor
{
    private PlayerInput input;

    protected int inventorySize = 40;
    public int InventorySize { get => inventorySize; }
    // FOV radius not in cells but the floating-point distance between player
    // and point in grid
    private int fovRadius = 15;

    private List<Cell> visibleCells = new List<Cell>();
    private List<Enemy> visibleEnemies = new List<Enemy>();
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
    public void RaiseInventoryChangeEvent() => OnInventoryChangeEvent?.Invoke();

    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        inventory = new List<Item>(inventorySize);
        movePath = new List<Cell>();
    }

    // Start is called before the first frame update
    private void Start() => input = GetComponent<PlayerInput>();

    // Request this actor's action and carry it out
    public override int Act()
    {
        if (movePath.Count > 0)
        {
            if (visibleEnemies.Count > 0)
            {
                GameLog.Send($"An enemy is nearby!", MessageColour.Red);
                movePath.Clear();
                return -1;
            }  

            Cell destination = movePath[0];
            movePath.RemoveAt(0);
            return new MoveAction(this, moveSpeed, destination).DoAction();
        }

        if (nextAction != null)
        {
            BaseAction ret = nextAction;
            // Clear action buffer
            nextAction = null;
            return ret.DoAction();
        }

        else return -1;
    }

    // Remove an item from this actor's inventory
    public override void RemoveItem(Item item)
    {
        base.RemoveItem(item);
        OnInventoryChangeEvent?.Invoke();
    }

    // Update the list of cells visible to this player
    public void UpdateVisibleCells(List<Cell> refreshed)
    {
        visibleCells.Clear();
        visibleEnemies.Clear();

        foreach (Cell c in refreshed)
            if (c.Visible) visibleCells.Add(c);

        // Also refresh list of visible enemies
        foreach (Cell c in visibleCells)
            if (c._actor is Enemy)
                visibleEnemies.Add((Enemy)c._actor);
    }

    #region AdvancedAttack

    public void StartAdvancedAttack()
    {
        input.PointTargetConfirmEvent += ConfirmAdvancedAttack;
        input.TargetCancelEvent += CancelAdvancedAttack;
    }

    public void ConfirmAdvancedAttack(Cell target)
    {
        if (target._actor == null)
            GameLog.Send("This feature isn't implemented...", MessageColour.Teal);
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
        GameLog.Send("You perish...", MessageColour.Purple);
    }
}

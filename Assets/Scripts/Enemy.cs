// Enemy behaviour handling
using UnityEngine;
using System.Collections.Generic;

public class Enemy : Actor
{
    // Requisite objects
    private SpriteRenderer spriteRenderer;

    public Player player;
    Actor target;

    // Awake is called when the first script instance is being loaded
    protected override void Awake()
    {
        maxHealth = 3;
        actorName = "Goblin";
        minDamage = 1;
        maxDamage = 2;
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        TurnController.instance.OnNPCTurnEvent += DecideAction;
        TurnController.instance.OnTurnChangeEvent += TurnUpdate;

        MoveToCell(level.GetRandomWalkable());
        spriteRenderer.enabled = cell.Visible;
    }

    // Changes made to this actor on a per-turn basis
    void TurnUpdate()
    {
        spriteRenderer.enabled = cell.Visible;
    }

    // Evaluate the situation and decide what to do next
    void DecideAction()
    {
        // Detect player if coming into player's view
        if (cell.Visible && target == null)
        {
            target = player;
            GameLog.Send($"The {actorName} notices you!", MessageColour.Red);
        }

        // Engage in combat
        if (target != null)
            PathMoveToTarget();
    }

    // Make a single move along a path towards a target
    void PathMoveToTarget()
    {
        List<Cell> path = level.pf.GetCellPath(Position, target.Position);
        if (path.Count > 0)
            TryMove(path[0]);
    }

    // Handle enemy death
    protected override void OnDeath()
    {
        base.OnDeath();
        TurnController.instance.OnNPCTurnEvent -= DecideAction;
        TurnController.instance.OnTurnChangeEvent -= TurnUpdate;
    }
}

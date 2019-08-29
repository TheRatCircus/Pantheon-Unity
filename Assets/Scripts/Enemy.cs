// Enemy behaviour handling
using UnityEngine;
using System.Collections.Generic;

public class Enemy : Actor
{
    // Requisite objects
    private SpriteRenderer spriteRenderer;

    Actor target;

    // Awake is called when the first script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = cell.Visible;
    }

    // Evaluate the situation and act
    public override int Act()
    {
        // Change visibility
        spriteRenderer.enabled = cell.Visible;

        // Detect player if coming into player's view
        if (cell.Visible && target == null)
        {
            target = Game.instance.player1;
            GameLog.Send($"{GameLog.GetSubject(this, true)} notices you!", MessageColour.Red);
        }

        // Engage in combat
        if (target != null)
            if (!level.AdjacentTo(cell, target._cell))
            {
                PathMoveToTarget();
                return moveSpeed;
            }
            else
            {
                TryToHit(target);
                return attackSpeed;
            }
        else
            return Game.TurnTime;
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
    }
}

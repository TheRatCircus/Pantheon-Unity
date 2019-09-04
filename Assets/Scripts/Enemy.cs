// Enemy behaviour handling
using UnityEngine;
using System.Collections.Generic;
using Pantheon.Actions;

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

    // Every time something happens, this NPC must refresh its visibility
    public void UpdateVisibility() => spriteRenderer.enabled = cell.Visible;

    // Evaluate the situation and act
    public override int Act()
    {
        // Detect player if coming into player's view
        if (cell.Visible && target == null)
        {
            target = Game.GetPlayer();
            GameLog.Send($"{GameLog.GetSubject(this, true)} notices you!", MessageColour.Red);
        }

        // Engage in combat
        if (target != null)
            if (!level.AdjacentTo(cell, target.Cell))
                PathMoveToTarget();
            else
                nextAction = new MeleeAction(this, attackTime, target);
        else
            nextAction = new WaitAction(this);

        BaseAction ret = nextAction;
        // Clear action buffer
        nextAction = null;
        return ret.DoAction();
    }

    // Make a single move along a path towards a target
    void PathMoveToTarget()
    {
        List<Cell> path = level.pf.GetCellPath(Position, target.Position);
        if (path.Count > 0)
            nextAction = new MoveAction(this, moveSpeed, path[0]);
    }

    // Handle enemy death
    protected override void OnDeath()
    {
        base.OnDeath();
        level.Enemies.Remove(this);
    }
}

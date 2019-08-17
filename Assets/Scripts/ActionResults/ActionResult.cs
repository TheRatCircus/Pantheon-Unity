// First class functionality for a the results of an action
using System.Collections.Generic;

public class ActionResult
{
    protected Actor actor;
    protected BaseAction action;

    public delegate void LineActionDelegate(List<Cell> line);

    // Constructor
    public ActionResult(Actor actor, BaseAction action)
    {
        this.actor = actor;
        this.action = action;
    }

    // Attempt to carry out the action
    public void TryAction()
    {
        if (action.NeedsLineTaret)
            RequestLineTarget();
        else if (action.NeedsActor)
        {
            action.DoAction(actor);
            CompleteAction();
        }
    }

    // If need be, request a line
    protected void RequestLineTarget()
    {
        if (actor is Player)
            ((Player)actor)._input.StartCoroutine(((Player)actor)._input.LineTarget(this));
        else if (actor is Enemy)
            throw new System.NotImplementedException("NPC attempted line targetting");
        else if (actor == null)
            throw new System.Exception("Actor not assigned to ActionResult");
    }

    // Perform a line action if prescribed by the action
    public virtual void DoLineAction(List<Cell> line)
    {
        action.DoAction(line);
        CompleteAction();
    }

    // Finish this action's result
    protected virtual void CompleteAction()
    {
        if (actor is Player)
            Game.instance.EndTurn();
    }
}

// Prescribes behaviours carried out ingame
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : ScriptableObject
{
    public ActionResult Result;
    public bool NeedsLineTaret;
    public bool NeedsActor;

    // Different overloads of DoAction for different needed parameters
    public virtual void DoAction() { }
    public virtual void DoAction(Actor actor) { }
    public virtual void DoAction(List<Cell> line) { }
}

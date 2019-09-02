// ScriptableObject wrapper for a LineProjAction
using UnityEngine;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "LineProjAction", menuName = "ActionWrappers/LineProjAction")]
public class LineProjActionWrapper : ActionWrapper
{
    public LineProjAction LineProjAction;

    // Get wrapped action
    public override BaseAction GetAction(Actor actor)
    {
        LineProjAction.Actor = actor;
        return LineProjAction;
    }
}
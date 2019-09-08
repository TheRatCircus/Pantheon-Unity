// ScriptableObject wrapper for a LineProjAction

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "LineProjAction", menuName = "ActionWrappers/LineProjAction")]
public class LineProjActionWrapper : ActionWrapper
{
    [SerializeField] private LineProjAction LineProjAction = null;

    // Get wrapped action
    public override BaseAction GetAction(Actor actor)
    {
        LineProjAction.Actor = actor;
        return LineProjAction;
    }
}
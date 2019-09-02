// ScriptableObject wrapper for a HealAction
using UnityEngine;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "HealAction", menuName = "ActionWrappers/HealAction")]
public class HealActionWrapper : ActionWrapper
{
    public HealAction HealAction;

    // Get wrapped action
    public override BaseAction GetAction(Actor actor)
    {
        HealAction.Actor = actor;
        return HealAction;
    }
}
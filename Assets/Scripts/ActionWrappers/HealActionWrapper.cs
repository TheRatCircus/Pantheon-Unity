// ScriptableObject wrapper for a HealAction

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "HealAction", menuName = "ActionWrappers/HealAction")]
public class HealActionWrapper : ActionWrapper
{
    [SerializeField] private HealAction HealAction = null;

    // Get wrapped action
    public override BaseAction GetAction(Actor actor)
    {
        HealAction.Actor = actor;
        return HealAction;
    }
}
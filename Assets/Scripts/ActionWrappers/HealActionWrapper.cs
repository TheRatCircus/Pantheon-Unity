// HealActionWrapper.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "HealAction", menuName = "ActionWrappers/HealAction")]
public class HealActionWrapper : ActionWrapper
{
    [SerializeField] private HealAction HealAction = null;

    public override BaseAction GetAction(Actor actor)
    {
        HealAction.Actor = actor;
        return HealAction;
    }
}
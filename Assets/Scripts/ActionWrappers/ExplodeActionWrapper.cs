// ExplodeActionWrapper.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "ExplodeAction", menuName = "ActionWrappers/ExplodeAction")]
public class ExplodeActionWrapper : ActionWrapper
{
    [SerializeField] private ExplodeAction explodeAction = null;

    public override BaseAction GetAction(Actor actor)
    {
        explodeAction.Actor = actor;
        return explodeAction;
    }
}
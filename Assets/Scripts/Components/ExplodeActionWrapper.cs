// ExplodeActionWrapper.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

namespace Pantheon
{
    [CreateAssetMenu(fileName = "ExplodeAction",
        menuName = "Pantheon/Actions/ExplodeAction")]
    public sealed class ExplodeActionWrapper : ActionWrapper
    {
        [SerializeField] private ExplodeAction explodeAction = null;

        public override Command GetAction(Actor actor)
        {
            explodeAction.Actor = actor;
            return explodeAction;
        }
    }
}

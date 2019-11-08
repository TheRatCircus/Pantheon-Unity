// LineProjActionWrapper.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

namespace Pantheon
{
    [CreateAssetMenu(fileName = "LineProjAction",
        menuName = "Pantheon/Actions/LineProjAction")]
    public sealed class LineProjActionWrapper : ActionWrapper
    {
        [SerializeField] private LineProjAction LineProjAction = null;

        public override Command GetAction(Actor actor)
        {
            LineProjAction.Actor = actor;
            return LineProjAction;
        }
    }
}

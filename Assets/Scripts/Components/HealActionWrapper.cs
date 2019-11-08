// HealActionWrapper.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Actions;

namespace Pantheon
{
    [CreateAssetMenu(fileName = "HealAction",
        menuName = "Pantheon/Actions/HealAction")]
    public sealed class HealActionWrapper : ActionWrapper
    {
        [SerializeField] private HealAction HealAction = null;

        public override Command GetAction(Actor actor)
        {
            HealAction.Actor = actor;
            return HealAction;
        }
    }
}

// WallTemplate.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS.Templates
{
    [CreateAssetMenu(fileName = "New Wall Template",
        menuName = "Pantheon/Templates/Wall")]
    public sealed class WallTemplate : Template
    {
        [SerializeField] private Blocking blocking = default;
        [SerializeField] private Position position = default;

        public override BaseComponent[] Unload()
        {
            return new BaseComponent[]
            {
                blocking,
                position
            };
        }
    }
}

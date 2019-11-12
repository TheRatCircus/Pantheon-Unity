// GroundTemplate.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS.Templates
{
    [CreateAssetMenu(fileName = "New GroundTemplate",
        menuName = "Pantheon/Templates/Ground")]
    public sealed class GroundTemplate : Template
    {
        [SerializeField] private UnityTile tile = default;
        [SerializeField] private Position position = default;
        [SerializeField] private Ground ground = default;

        public override BaseComponent[] Unload()
        {
            return new BaseComponent[]
            {
                tile.DeepClone(),
                position.DeepClone(),
                ground.DeepClone()
            };
        }
    }
}

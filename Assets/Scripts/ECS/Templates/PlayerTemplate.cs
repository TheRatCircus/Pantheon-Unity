// PlayerTemplate.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS.Templates
{
    [CreateAssetMenu(fileName = "New Player Template",
        menuName = "Pantheon/Templates/Player")]
    public sealed class PlayerTemplate : Template
    {
        [SerializeField] private UnityGameObject gameObject = default;
        [SerializeField] private Actor actor = default;
        [SerializeField] private Player player = default;
        [SerializeField] private Blocking blocking = default;
        [SerializeField] private Position position = default;

        public override BaseComponent[] Unload()
        {
            return new BaseComponent[]
            {
                gameObject,
                actor,
                player,
                blocking,
                position
            };
        }
    }
}

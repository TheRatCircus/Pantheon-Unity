// PlayerTemplate.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS.Templates
{
    [CreateAssetMenu(fileName = "PlayerTemplate",
        menuName = "Pantheon/Templates/Player")]
    public sealed class PlayerTemplate : Template
    {
        [SerializeField] private GameObjectSprite sprite = default;
        [SerializeField] private Actor actor = default;
        [SerializeField] private Player player = default;
        [SerializeField] private UnityGameObject gameObj = default;
        [SerializeField] private Blocking blocking = default;
        [SerializeField] private Position position = default;

        public override BaseComponent[] Unload()
        {
            return new BaseComponent[]
            {
                sprite,
                actor,
                player,
                gameObj,
                blocking,
                position
            };
        }
    }
}

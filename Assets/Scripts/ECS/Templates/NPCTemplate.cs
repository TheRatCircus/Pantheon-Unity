// NPCTemplate.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS.Templates
{
    [CreateAssetMenu(fileName = "New NPC Template",
        menuName = "Pantheon/Templates/NPC")]
    public sealed class NPCTemplate : Template
    {
        [SerializeField] private UnityGameObject gameObject = default;
        [SerializeField] private Actor actor = default;
        [SerializeField] private AI ai = default;
        [SerializeField] private Blocking blocking = default;
        [SerializeField] private Position position = default;

        public override BaseComponent[] Unload()
        {
            return new BaseComponent[]
            {
                gameObject,
                actor,
                ai,
                blocking,
                position
            };
        }
    }
}


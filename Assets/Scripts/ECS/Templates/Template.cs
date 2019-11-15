// Template.cs
// Jerome Martina

using Pantheon.ECS.Components;
using UnityEngine;

namespace Pantheon.ECS.Templates
{
    public abstract class Template : ScriptableObject
    {
        [SerializeField] private string entityName = "DEFAULT_ENTITY_NAME";
        public string EntityName => entityName;

        [SerializeField] private Sprite sprite = default;
        public Sprite Sprite => sprite;
        [SerializeField] private RuleTile tile = default;
        public RuleTile Tile => tile;

        [SerializeField] private EntityArchetype archetype = default;

        public EntityArchetype Archetype => archetype;
        public abstract BaseComponent[] Unload();
    }
}

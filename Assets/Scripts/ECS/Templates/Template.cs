// Template.cs
// Jerome Martina

using UnityEngine;
using Pantheon.ECS.Components;

namespace Pantheon.ECS.Templates
{
    public abstract class Template : ScriptableObject
    {
        [SerializeField] private string entityName = "DEFAULT_ENTITY_NAME";
        public string Name => entityName;
        [SerializeField] private EntityArchetype archetype = default;

        public EntityArchetype Archetype => archetype;
        public abstract BaseComponent[] Unload();
    }
}

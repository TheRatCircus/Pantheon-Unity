// EntityTemplate.cs
// Jerome Martina

using Pantheon.ECS.Components;
using System.Collections.Generic;

namespace Pantheon.ECS.Templates
{
    public sealed class EntityTemplate
    {
        public string ID { get; private set; } = "DEFAULT_TEMPLATE_ID";
        public string EntityName { get; private set; } = "DEFAULT_ENTITY_NAME";
        public EntityArchetype Archetype { get; private set; } = default;
        public Dictionary<string, BaseComponent> Components { get; set; }

        public bool ContainsComponent<T>() where T : BaseComponent
        {
            return Components.ContainsKey(typeof(T).Name);
        }
    }
}

// EntityTemplate.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.ECS.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.ECS.Templates
{
    public sealed class EntityTemplate
    {
        public string ID { get; private set; } = "DEFAULT_TEMPLATE_ID";
        public string EntityName { get; private set; } = "DEFAULT_ENTITY_NAME";
        public EntityArchetype Archetype { get; private set; } = default;
        public Dictionary<string, BaseComponent> Components { get; private set; }

        public Sprite Sprite { get; private set; }
        public RuleTile Tile { get; private set; }

        public EntityTemplate(string id, string name)
        {
            ID = id;
            EntityName = name;
        }

        [JsonConstructor]
        public EntityTemplate(Dictionary<string, BaseComponent> components)
        {
            Components = components;
        }

        public bool TryGetComponent<T>(out T ret)
            where T : BaseComponent
        {
            if (!Components.TryGetValue(typeof(T).Name, out BaseComponent c))
            {
                ret = null;
                return false;
            }
            else
            {
                ret = (T)c;
                return true;
            }
        }
    }
}

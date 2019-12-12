// EntityTemplate.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.ECS.Components;
using UnityEngine;
using static System.Environment;

namespace Pantheon.ECS
{
    public sealed class EntityTemplate
    {
        public string ID { get; private set; } = "DEFAULT_TEMPLATE_ID";
        public string EntityName { get; private set; } = "DEFAULT_ENTITY_NAME";
        public Sprite Sprite { get; private set; } = default;
        public RuleTile Tile { get; private set; } = default;
        public EntityComponent[] Components { get; private set; }

        [JsonConstructor]
        public EntityTemplate(string id, string entityName, Sprite sprite,
            params EntityComponent[] components)
        {
            ID = id;
            EntityName = entityName;
            Sprite = sprite;
            Components = components;
            if (Tile == null)
            {
                Tile = ScriptableObject.CreateInstance<RuleTile>();
                Tile.m_DefaultSprite = Sprite;
            }
        }

        public bool TryGetComponent<T>(out T component)
            where T : EntityComponent
        {
            foreach (EntityComponent ec in Components)
            {
                if (ec is T)
                {
                    component = (T)ec;
                    return true;
                }
            }

            component = null;
            return false;
        }

        public bool HasComponent<T>() where T : EntityComponent
        {
            foreach (EntityComponent ec in Components)
                if (ec is T)
                    return true;

            return false;
        }

        public override string ToString()
        {
            string ret = $"{EntityName}{NewLine}";
            foreach (EntityComponent bc in Components)
            {
                ret += bc.ToString();
                ret += NewLine;
            }
            return ret;
        }
    }
}

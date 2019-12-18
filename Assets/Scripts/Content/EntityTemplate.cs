// EntityTemplate.cs
// Jerome Martina

using static System.Environment;
using Pantheon.Components;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Tilemaps;

namespace Pantheon.Content
{
    public sealed class EntityTemplate
    {
        public string ID { get; private set; } = "DEFAULT_TEMPLATE_ID";
        public string EntityName { get; private set; } = "DEFAULT_ENTITY_NAME";
        public Sprite Sprite { get; private set; } = default;
        public Tile Tile { get; private set; } = default;
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
                Tile = ScriptableObject.CreateInstance<Tile>();
                Tile.sprite = Sprite;
            }
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

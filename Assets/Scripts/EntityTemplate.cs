// EntityTemplate.cs
// Jerome Martina

using static System.Environment;
using Pantheon.Components;
using UnityEngine;

namespace Pantheon
{
    public sealed class EntityTemplate
    {
        public string ID { get; set; } = "DEFAULT_TEMPLATE_ID";
        public string EntityName { get; set; } = "DEFAULT_ENTITY_NAME";
        public Sprite Sprite { get; set; } = default;
        public RuleTile Tile { get; set; } = default;
        public EntityComponent[] Components { get; set; }

        public EntityTemplate(params EntityComponent[] components)
        {
            Components = components;
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

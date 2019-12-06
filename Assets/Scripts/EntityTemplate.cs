// EntityTemplate.cs
// Jerome Martina

using static System.Environment;
using Pantheon.Components;
using UnityEngine;

namespace Pantheon
{
    public sealed class EntityTemplate
    {
        public string Name { get; set; } = "DEFAULT_TEMPLATE_NAME";
        public Sprite Sprite { get; set; } = default;
        public EntityComponent[] Components { get; set; }

        public EntityTemplate(params EntityComponent[] components)
        {
            Components = components;
        }

        public override string ToString()
        {
            string ret = $"{Name}{NewLine}";
            foreach (EntityComponent bc in Components)
            {
                ret += bc.ToString();
                ret += NewLine;
            }
            return ret;
        }
    }
}

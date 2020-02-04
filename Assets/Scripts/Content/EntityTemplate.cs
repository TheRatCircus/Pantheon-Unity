// EntityTemplate.cs
// Jerome Martina

using static System.Environment;
using Pantheon.Components.Entity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.Content
{
    public sealed class EntityTemplate
    {
        public string ID { get; set; } = "DEFAULT_TEMPLATE_ID";
        public string EntityName { get; set; } = "DEFAULT_ENTITY_NAME";
        public EntityFlag Flags { get; set; }
        public Sprite Sprite { get; set; }
        public Tile Tile { get; set; }
        public EntityComponent[] Components { get; set; }

        public EntityTemplate() { }

        public EntityTemplate(Entity entity)
        {
            EntityName = entity.Name;
            Sprite = entity.Flyweight.Sprite;
            Components = new EntityComponent[entity.Components.Count];
            int i = 0;
            foreach (EntityComponent ec in entity.Components.Values)
            {
                Components[i++] = ec.Clone(false);
            }
            if (entity.Flyweight.Tile != null)
                Tile = entity.Flyweight.Tile;
            else
            {
                Tile = ScriptableObject.CreateInstance<Tile>();
                Tile.sprite = Sprite;
            }
        }

        public bool TryGetComponent<T>(out T ret) where T : EntityComponent
        {
            foreach (EntityComponent ec in Components)
            {
                if (ec.GetType() == typeof(T))
                {
                    ret = (T)ec;
                    return true;
                }
            }
            ret = null;
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

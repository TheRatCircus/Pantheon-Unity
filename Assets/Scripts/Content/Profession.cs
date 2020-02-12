// Profession.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Components.Entity;
using Pantheon.Serialization.Json.Converters;

namespace Pantheon.Content
{
    public sealed class Profession
    {
        public string ID { get; set; } = "DEFAULT_PROFESSION_ID";
        public string Name { get; set; } = "DEFAULT_PROFESSION_NAME";
        public EntityComponent[] Components { get; set; }
        public bool OverwriteAI { get; set; }
        [JsonConverter(typeof(TemplateArrayConverter))]
        public EntityTemplate[] Wielded { get; set; }
        [JsonConverter(typeof(TemplateArrayConverter))]
        public EntityTemplate[] Inventory { get; set; }

        public bool Apply(Entity entity)
        {
            if (Components != null)
                foreach (EntityComponent ec in Components)
                    if (ec != null && entity.HasComponent(ec.GetType()))
                        entity.AddComponent(ec.Clone(false));

            if (!entity.HasComponent<Actor>())
                return false;

            string name = "";
            if (entity.TryGetComponent(out Species species))
                name += $"{species.SpeciesDef.Name} ";
            name += Name;

            if (entity.TryGetComponent(out Wield wield))
            {
                Entity[] items = new Entity[Wielded.Length];
                for (int i = 0; i < Wielded.Length; i++)
                    items[i] = new Entity(Wielded[i]);
                wield.ForceWield(items);
            }

            if (entity.TryGetComponent(out Inventory inv))
            {
                foreach (EntityTemplate et in Inventory)
                    if (et != null) inv.AddItem(new Entity(et));
            }

            if (OverwriteAI)
            {
                if (!TryGetComponent(out AI profAi))
                    throw new System.Exception(
                        $"Profession {ID} does not have an AI component.");

                if (entity.TryGetComponent(out AI ai))
                {
                    ai.Utilities = profAi.Utilities;
                    ai.Peaceful = profAi.Peaceful;
                }
                else
                    entity.AddComponent(profAi.Clone(false));
            }

            return true;
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
    }
}

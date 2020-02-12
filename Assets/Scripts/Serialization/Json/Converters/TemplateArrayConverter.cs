// TemplateArrayConverter.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Content;
using System;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class TemplateArrayConverter : JsonConverter<EntityTemplate[]>
    {
        public override EntityTemplate[] ReadJson(JsonReader reader, Type objectType,
            EntityTemplate[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string[] ids = serializer.Deserialize<string[]>(reader);

            if (ids == null)
                return null;

            EntityTemplate[] templates = new EntityTemplate[ids.Length];
            
            for (int i = 0; i < ids.Length; i++)
                templates[i] = Assets.GetTemplate(ids[i]);

            return templates;
        }

        public override void WriteJson(JsonWriter writer, EntityTemplate[] value,
            JsonSerializer serializer)
        {
            string[] ids = new string[value.Length];

            for (int i = 0; i < ids.Length; i++)
                ids[i] = value[i].ID;

            serializer.Serialize(writer, ids);
        }
    }
}

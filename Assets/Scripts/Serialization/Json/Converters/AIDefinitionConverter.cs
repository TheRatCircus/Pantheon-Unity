// AIDefinitionConverter.cs
// Jerome Martina

using System;
using Newtonsoft.Json;
using Pantheon.Content;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class AIDefinitionConverter : JsonConverter<AIDefinition>
    {
        public override AIDefinition ReadJson(
            JsonReader reader, Type objectType, AIDefinition existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return Assets.AI[(string)reader.Value];
        }

        public override void WriteJson(JsonWriter writer, AIDefinition value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ID);
        }
    }
}

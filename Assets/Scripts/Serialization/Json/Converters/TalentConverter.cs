// TalentConverter.cs
// Jerome Martina

using Newtonsoft.Json;
using System;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class TalentConverter : JsonConverter<Talent>
    {
        public override Talent ReadJson(JsonReader reader, Type objectType,
            Talent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Assets.Talents[(string)reader.Value];
        }

        public override void WriteJson(JsonWriter writer, Talent value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ID);
        }
    }
}

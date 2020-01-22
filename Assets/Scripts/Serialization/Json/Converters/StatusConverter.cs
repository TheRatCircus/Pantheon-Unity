// StatusConverter.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Components.Entity.Statuses;
using Pantheon.Core;
using System;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class StatusConverter : JsonConverter<StatusDefinition>
    {
        private AssetLoader loader;

        public StatusConverter(AssetLoader loader = null)
            => this.loader = loader;

        public override StatusDefinition ReadJson(JsonReader reader,
            Type objectType, StatusDefinition existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return StatusDefinition.statuses[(string)reader.Value];
        }

        public override void WriteJson(JsonWriter writer, StatusDefinition value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ID);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

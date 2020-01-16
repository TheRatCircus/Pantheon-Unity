// TerrainConverter.cs
// Jerome Martina

using System;
using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.Core;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class TerrainConverter : JsonConverter<TerrainDefinition>
    {
        private AssetLoader loader;

        public override bool CanRead => true;
        public override bool CanWrite => true;

        public TerrainConverter() { }

        public TerrainConverter(AssetLoader loader) => this.loader = loader;

        public override TerrainDefinition ReadJson(JsonReader reader,
            Type objectType, TerrainDefinition existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            string id = (string)reader.Value;
            if (id == null)
                return null;
            else
                return loader.Load<TerrainDefinition>(id);
        }

        public override void WriteJson(JsonWriter writer,
            TerrainDefinition value, JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }
    }
}

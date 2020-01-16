// SpeciesDefinitionConverter.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.Core;
using System;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class SpeciesDefinitionConverter : JsonConverter<SpeciesDefinition>
    {
        private AssetLoader loader;

        public SpeciesDefinitionConverter(AssetLoader loader = null)
            => this.loader = loader;

        public override void WriteJson(JsonWriter writer, SpeciesDefinition value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ID);
        }

        public override SpeciesDefinition ReadJson(JsonReader reader, Type objectType,
            SpeciesDefinition existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return loader.LoadSpeciesDef((string)reader.Value);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

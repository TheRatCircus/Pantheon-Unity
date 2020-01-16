// BodyPartConverter.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.Core;
using System;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class BodyPartConverter : JsonConverter<BodyPart>
    {
        private AssetLoader loader;

        public BodyPartConverter(AssetLoader loader = null) => this.loader = loader;

        public override BodyPart ReadJson(JsonReader reader, Type objectType,
            BodyPart existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return loader.LoadBodyPart(reader.Value as string);
        }

        public override void WriteJson(JsonWriter writer, BodyPart value, 
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ID);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

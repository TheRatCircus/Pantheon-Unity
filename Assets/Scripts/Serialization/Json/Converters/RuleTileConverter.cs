
using Newtonsoft.Json;
using Pantheon.Core;
using System;
using UnityEngine;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class RuleTileConverter : JsonConverter<RuleTile>
    {
        public AssetLoader Loader { get; set; }

        public RuleTileConverter() { }

        public RuleTileConverter(AssetLoader loader) => Loader = loader;

        public override RuleTile ReadJson(JsonReader reader,
            Type objectType, RuleTile existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            // Tiles are generated programatically during template construction
            return null;
        }

        public override void WriteJson(JsonWriter writer, RuleTile value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

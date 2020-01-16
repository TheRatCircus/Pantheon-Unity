// SpriteConverter.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Core;
using System;
using UnityEngine;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class SpriteConverter : JsonConverter<Sprite>
    {
        private AssetLoader loader;

        public SpriteConverter(AssetLoader loader = null)
            => this.loader = loader;

        public override Sprite ReadJson(JsonReader reader,
            Type objectType, Sprite existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return loader.Load<Sprite>((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, Sprite value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

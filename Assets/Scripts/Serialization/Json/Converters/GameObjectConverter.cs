using System;
using Newtonsoft.Json;
using Pantheon.Core;
using UnityEngine;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class GameObjectConverter : JsonConverter<GameObject>
    {
        private AssetLoader loader;

        public GameObjectConverter(AssetLoader loader = null)
            => this.loader = loader;

        public override GameObject ReadJson(JsonReader reader, Type objectType,
            GameObject existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return loader.Load<GameObject>((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, GameObject value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

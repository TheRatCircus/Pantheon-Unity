// AudioClipConverter.cs
// Jerome Martina

using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class AudioClipConverter : JsonConverter<AudioClip>
    {
        public override AudioClip ReadJson(JsonReader reader, Type objectType,
            AudioClip existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;
            return s != null ? Assets.Audio[s] : null;
        }

        public override void WriteJson(JsonWriter writer, AudioClip value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

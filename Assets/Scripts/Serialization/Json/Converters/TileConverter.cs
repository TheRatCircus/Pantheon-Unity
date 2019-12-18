// TileConverter.cs
// Jerome Martina

using System;
using Newtonsoft.Json;
using UnityEngine.Tilemaps;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class TileConverter : JsonConverter<Tile>
    {
        public override Tile ReadJson(JsonReader reader, Type objectType,
            Tile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Tiles are generated programatically during template construction
            return null;
        }

        public override void WriteJson(JsonWriter writer, Tile value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.name);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

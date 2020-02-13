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
            string s = (string)reader.Value;
            if (s == null)
                return null;
            else
                return Assets.GetTile<Tile>((string)reader.Value);
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

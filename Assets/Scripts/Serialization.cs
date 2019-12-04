// Serialization.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pantheon.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pantheon
{
    public static class Serialization
    {
        public static readonly SerializationBinder _builderStepBinder
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    typeof(Fill),
                    typeof(RandomFill)
                }
            };

        public static readonly SerializationBinder _entityBinder
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                }
            };
    }

    public sealed class SerializationBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public void BindToName(Type serializedType, out string assemblyName,
            out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }
    }

    public interface IAssetLoadConverter
    {
        AssetLoader Loader { get; set; }
    }

    public sealed class SpriteConverter : JsonConverter<Sprite>, IAssetLoadConverter
    {
        public AssetLoader Loader { get; set; }

        public override Sprite ReadJson(JsonReader reader,
            Type objectType, Sprite existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return Loader.Load<Sprite>((string)reader.Value);
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

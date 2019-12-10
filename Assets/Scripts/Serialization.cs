// Serialization.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pantheon.Components;
using Pantheon.Core;
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
                    typeof(Actor),
                    typeof(AI),
                    typeof(Health),
                    typeof(Melee),
                    typeof(Species)
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

    public interface IAssetLoaderModule
    {
        AssetLoader Loader { get; set; }
    }

    public sealed class SpriteConverter : JsonConverter<Sprite>, IAssetLoaderModule
    {
        public AssetLoader Loader { get; set; }

        public SpriteConverter() { }

        public SpriteConverter(AssetLoader loader) => Loader = loader;

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

    public sealed class SpeciesConverter : JsonConverter<Species>
    {
        public AssetLoader Loader { get; set; }

        public SpeciesConverter() { }

        public SpeciesConverter(AssetLoader loader) => Loader = loader;

        public override void WriteJson(JsonWriter writer, Species value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.SpeciesDef.ID);
        }

        public override Species ReadJson(JsonReader reader, Type objectType,
            Species existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            Species species = reader.Value as Species;
            string defID = species.SpeciesDef.ID; // ID-only species def here
            species.SpeciesDef = Loader.LoadSpeciesDef(defID); // Fully-populated def
            return species;
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }

    public sealed class SpeciesDefinitionConverter : JsonConverter<SpeciesDefinition>
    {
        public AssetLoader Loader { get; set; }

        public SpeciesDefinitionConverter() { }

        public SpeciesDefinitionConverter(AssetLoader loader) => Loader = loader;

        public override void WriteJson(JsonWriter writer, SpeciesDefinition value,
            JsonSerializer serializer)
        {
            writer.WriteValue(value.ID);
        }

        public override SpeciesDefinition ReadJson(JsonReader reader, Type objectType,
            SpeciesDefinition existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return Loader.LoadSpeciesDef((string)reader.Value);
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

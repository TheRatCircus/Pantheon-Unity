
using Newtonsoft.Json;
using Pantheon.Components;
using Pantheon.Core;
using System;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class SpeciesConverter : JsonConverter<Species>
    {
        public AssetLoader Loader { get; set; }

        public SpeciesConverter() { }

        public SpeciesConverter(AssetLoader loader) => Loader = loader;

        public override void WriteJson(JsonWriter writer, Species value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override Species ReadJson(JsonReader reader, Type objectType,
            Species existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
    }
}

// WorldPlanConverter.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Pantheon.Gen;

namespace Pantheon.Serialization.Json.Converters
{
    public sealed class WorldPlanConverter: JsonConverter<Dictionary<string, Builder>>
    {
        public override Dictionary<string, Builder> ReadJson(
            JsonReader reader,
            Type objectType,
            Dictionary<string, Builder> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            string[] ids = serializer.Deserialize<string[]>(reader);

            if (ids == null)
                return null;

            Dictionary<string, Builder> ret = new Dictionary<string, Builder>(ids.Length);

            for (int i = 0; i < ids.Length; i++)
            {
                Builder builder = Assets.Builders[ids[i]];
                ret.Add(builder.ID, builder);
            }
            
            return ret;
        }

        public override void WriteJson(
            JsonWriter writer,
            Dictionary<string, Builder> value,
            JsonSerializer serializer)
        {
            string[] ids = value.Keys.ToArray();
            serializer.Serialize(writer, ids);
        }
    }
}

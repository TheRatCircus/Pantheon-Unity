// SpeciesDefinition.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Pantheon.Content
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BodyPlan : byte
    {
        None,
        Humanoid,
        Canid,
        Avian
    }

    public sealed class SpeciesDefinition
    {
        public string ID { get; set; } = "DEFAULT_SPECIES_ID";
        public string Name { get; set; } = "DEFAULT_SPECIES_NAME";
        public Sprite Sprite { get; set; }
        public BodyPart[] Parts { get; set; }
        public BodyPlan BodyPlan { get; set; }
        // TODO: Components

        public override string ToString() => Name;
    }
}

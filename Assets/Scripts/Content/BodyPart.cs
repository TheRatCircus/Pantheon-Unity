// BodyPart.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Components.Entity;

namespace Pantheon.Content
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BodyPartType
    {
        Torso,
        Head,
        Arms,
        Legs,
        Teeth,
        Wings
    }

    [System.Serializable]
    public sealed class BodyPart
    {
        public string ID { get; set; } = "DEFAULT_BODYPART_ID";
        public string Name { get; set; } = "DEFAULT_BODYPART_NAME";
        public BodyPartType Type { get; set; }
        public int MoveSpeedModifier { get; set; }
        public SpeciesDefinition Species { get; set; }
        public Melee Melee { get; set; }
    }
}

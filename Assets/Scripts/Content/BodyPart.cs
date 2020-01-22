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
        public string ID { get; private set; } = "DEFAULT_BODYPART_ID";
        public string Name { get; private set; } = "DEFAULT_BODYPART_NAME";
        public BodyPartType Type { get; private set; }
        public int MoveSpeedModifier { get; private set; }
        public SpeciesDefinition Species { get; private set; }
        public Melee Melee { get; private set; }

        public BodyPart() { }

        public BodyPart(string id) => ID = id;

        [JsonConstructor]
        public BodyPart(string id, string name, BodyPartType type,
            int moveSpeedModifier, SpeciesDefinition species, Melee melee)
        {
            ID = id;
            Name = name;
            Type = type;
            MoveSpeedModifier = moveSpeedModifier;
            Species = species;
            Melee = melee;
        }
    }
}

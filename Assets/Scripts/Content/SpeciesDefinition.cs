// SpeciesDefinition.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Pantheon.Content
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BodyPattern
    {
        // TODO: BodyPattern.None
        Humanoid,
        Canid,
        Avian
    }

    [System.Serializable]
    public sealed class SpeciesDefinition
    {
        public string ID { get; set; } = "DEFAULT_SPECIES_ID";
        public string Name { get; set; } = "DEFAULT_SPECIES_NAME";
        public Sprite Sprite { get; set; }
        public BodyPart[] Parts { get; set; }
        public BodyPattern BodyPattern { get; set; }
        // TODO: Components

        // TODO: Kill this constructor, use object initializers instead
        [JsonConstructor]
        public SpeciesDefinition(string id, string name, Sprite sprite,
            BodyPattern bodyPattern, params BodyPart[] parts)
        {
            ID = id;
            Name = name;
            Sprite = sprite;
            Parts = parts;
            BodyPattern = bodyPattern;
        }

        /// <summary>
        /// Construct an ID-only species for use by a JSON writer.
        /// </summary>
        /// <param name="id">Asset ID of the species definition.</param>
        public SpeciesDefinition(string id) => ID = id;

        public override string ToString() => ID;
    }
}

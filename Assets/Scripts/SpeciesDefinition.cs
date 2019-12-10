// SpeciesDefinition.cs
// Jerome Martina

using Newtonsoft.Json;
using UnityEngine;

namespace Pantheon
{
    public sealed class SpeciesDefinition
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public Sprite Sprite { get; private set; }
        public BodyPart[] Parts { get; private set; }

        [JsonConstructor]
        public SpeciesDefinition(string id, string name, Sprite sprite, BodyPart[] parts)
        {
            ID = id;
            Name = name;
            Sprite = sprite;
            Parts = parts;
        }

        /// <summary>
        /// Construct an ID-only species for use by a JSON writer.
        /// </summary>
        /// <param name="id">Asset ID of the species definition.</param>
        public SpeciesDefinition(string id) => ID = id;

        public override string ToString() => ID;
    }
}

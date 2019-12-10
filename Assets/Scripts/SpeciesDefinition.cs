// SpeciesDefinition.cs
// Jerome Martina

using Newtonsoft.Json;
using UnityEngine;

namespace Pantheon
{
    [System.Serializable]
    public sealed class SpeciesDefinition
    {
        [SerializeField] private string id = "DEFAULT_SPECIES_ID";
        [SerializeField] private string name = "DEFAULT_SPECIES_NAME";
        [SerializeField] private Sprite sprite;

        public string ID => id;
        public string Name => name;
        public Sprite Sprite => sprite;
        public BodyPart[] Parts { get; }

        [JsonConstructor]
        public SpeciesDefinition(string id, string name, Sprite sprite, BodyPart[] parts)
        {
            this.id = id;
            this.name = name;
            this.sprite = sprite;
            Parts = parts;
        }

        /// <summary>
        /// Construct an ID-only species for use by a JSON writer.
        /// </summary>
        /// <param name="id">Asset ID of the species definition.</param>
        public SpeciesDefinition(string id) => this.id = id;

        public override string ToString() => ID;
    }
}

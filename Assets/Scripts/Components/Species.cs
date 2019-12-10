// Species.cs
// Jerome Martina

using Newtonsoft.Json;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Species : EntityComponent
    {
        [JsonConverter(typeof(SpeciesDefinitionConverter))]
        public SpeciesDefinition SpeciesDef { get; set; }

        public Species(SpeciesDefinition def)
        {
            SpeciesDef = def;
        }

        public override EntityComponent Clone()
        {
            return new Species(SpeciesDef);
        }

        public override string ToString()
        {
            return $"Entity species: {SpeciesDef.Name}";
        }
    }
}

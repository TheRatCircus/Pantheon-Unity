// Species.cs
// Jerome Martina

using Pantheon.Content;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Species : EntityComponent
    {
        public SpeciesDefinition SpeciesDef { get; set; }

        public Species(SpeciesDefinition def)
        {
            SpeciesDef = def;
        }

        public override EntityComponent Clone(bool full)
        {
            return new Species(SpeciesDef);
        }

        public override string ToString()
        {
            return $"Entity species: {SpeciesDef.Name}";
        }
    }
}

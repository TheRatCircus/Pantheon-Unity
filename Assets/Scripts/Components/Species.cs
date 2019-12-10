// Species.cs
// Jerome Martina

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

        public override EntityComponent Clone()
        {
            return new Species(SpeciesDef);
        }
    }
}

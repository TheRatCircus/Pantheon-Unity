// Relic.cs
// Jerome Martina

namespace Pantheon.Components.Entity
{
    [System.Serializable]
    public sealed class Relic : EntityComponent
    {
        public string Name { get; set; }

        public override EntityComponent Clone(bool full)
        {
            return new Relic() { Name = Name };
        }
    }
}

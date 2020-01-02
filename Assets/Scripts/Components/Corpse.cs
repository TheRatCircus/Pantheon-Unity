// Corpse.cs
// Jerome Martina

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Corpse : EntityComponent
    {
        public Entity Original { get; set; } // Entity which left this corpse
        public bool Skeletal { get; set; }

        public override EntityComponent Clone(bool full)
        {
            return new Corpse()
            {
                Original = Original,
                Skeletal = Skeletal
            };
        }
    }
}

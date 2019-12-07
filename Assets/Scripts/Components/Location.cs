// EntityComponent.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Location : EntityComponent
    {
        public Cell Cell { get; set; }
        public Level Level { get; set; }
    }
}

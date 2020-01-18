// EntityComponent.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Location : EntityComponent
    {
        public Level Level { get; set; } = null;

        public Location() { }

        public Location(Level level)
        {
            Level = level;
        }

        public override EntityComponent Clone(bool full)
        {
            if (full)
                return new Location(Level);
            else
                return new Location();
        }
    }
}

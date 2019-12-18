// EntityComponent.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Location : EntityComponent
    {
        public Cell Cell { get; set; } = null;
        public Level Level { get; set; } = null;

        public Location() { }

        public Location(Cell cell, Level level)
        {
            Cell = cell;
            Level = level;
        }

        public override EntityComponent Clone(bool full)
        {
            if (full)
                return new Location(Cell, Level);
            else
                return new Location();
        }
    }
}

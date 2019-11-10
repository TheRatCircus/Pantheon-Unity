// Position.cs
// Jerome Martina

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Position : BaseComponent
    {
        public Level Level { get; set; }
        public Cell Cell { get; set; }

        public Level DestinationLevel { get; set; }
        public Cell DestinationCell { get; set; }

        public Position(Level level, Cell cell)
        {
            Level = level;
            Cell = cell;
        }

        public void SetDestination(Level destinationLevel, Cell destinationCell)
        {
            DestinationLevel = destinationLevel;
            DestinationCell = destinationCell;
        }
    }
}

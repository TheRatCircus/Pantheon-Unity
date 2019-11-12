// Position.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Position : BaseComponent
    {
        public Level Level { get; set; }
        public Cell Cell { get; set; }

        public Level DestinationLevel { get; set; }
        public Cell DestinationCell { get; set; }

        public Position() { }

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

        public override BaseComponent Clone()
        {
            return new Position();
        }

        public override string ToString()
        {
            string current = $"{(Cell != null ? Cell.ToString() : "No cell")};";
            string destination = $"destination: {(DestinationCell != null ? DestinationCell.ToString() : "Nowhere")}";
            return $"{current} {destination}";
        }
    }
}

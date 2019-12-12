// EntityComponent.cs
// Jerome Martina

using Pantheon.World;
using UnityEngine;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Location : EntityComponent
    {
        public Cell Cell { get; set; } = null;
        public Level Level { get; set; } = null;

        public LocationCommand Cmd { get; set; }
        public bool Moving => Cmd.cell != Cell;

        public Location() { }

        public Location(Cell cell, Level level)
        {
            Cell = cell;
            Level = level;
        }

        public bool Walk(Vector2Int dir)
        {
            if (Level.TryGetCell(Cell.Position + dir, out Cell cell)
                && Cell.Walkable(cell)) 
            {
                Cmd = new LocationCommand(Level, cell);
                return true;
            }
            else return false;
        }

        public void Move(Level level, Cell cell)
            => Cmd = new LocationCommand(level, cell);

        public void ResetCommand() => Cmd = new LocationCommand(Level, Cell);

        public override EntityComponent Clone() => new Location(Cell, Level);
    }

    public struct LocationCommand
    {
        public readonly Cell cell;
        public readonly Level level;

        public LocationCommand(Level level, Cell cell)
        {
            this.level = level;
            this.cell = cell;
        }
    }
}

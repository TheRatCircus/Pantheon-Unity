// IPlayer.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Core
{
    public interface IPlayer
    {
        Entity Entity { get; }
        InputMode Mode { get; set; }
        void RecalculateVisible(IEnumerable<Cell> cells);
        HashSet<Entity> VisibleActors { get; }
        InputMode RequestCell(out Cell cell, int range);
        InputMode RequestLine(out List<Cell> line, int range);
    }
}

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
        void UpdateVisibles(IEnumerable<Cell> cells);
        HashSet<Cell> VisibleCells { get; }
        InputMode RequestCell(out Cell cell, int range);
        InputMode RequestLine(out List<Cell> line, int range);
    }
}

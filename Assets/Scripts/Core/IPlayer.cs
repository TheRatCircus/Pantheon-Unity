// IPlayer.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Core
{
    public interface IPlayer
    {
        Entity Entity { get; }
        InputMode Mode { get; set; }
        void RecalculateVisible(IEnumerable<Vector2Int> cells);
        HashSet<Entity> VisibleActors { get; }
        InputMode RequestCell(out Cell cell, int range);
        InputMode RequestLine(out List<Cell> line, int range);
    }
}

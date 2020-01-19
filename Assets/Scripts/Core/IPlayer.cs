// IPlayer.cs
// Jerome Martina

using Pantheon.Utils;
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
        InputMode RequestCell(out Vector2Int cell, int range);
        InputMode RequestLine(out Line line, int range);
        Vector2Int GetTargetedAdjacent();
    }
}

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
        void UpdateVisibles(IEnumerable<Vector2Int> cells);
        List<Vector2Int> VisibleCells { get; }
        InputMode RequestCell(out Vector2Int cell, int range);
        InputMode RequestLine(out Line line, int range);
    }
}

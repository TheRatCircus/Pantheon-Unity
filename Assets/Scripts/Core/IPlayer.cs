// IPlayer.cs
// Jerome Martina

using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Core
{
    public interface IPlayer
    {
        Entity Entity { get; }
        InputMode Mode { get; set; }
        InputMode RequestCell(out Vector2Int cell, int range);
        InputMode RequestLine(out Line line, int range);
        Vector2Int GetTargetedAdjacent();
    }
}

// Zone.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// A grouping of 9 levels, usually ruled by a boss.
    /// </summary>
    public sealed class Zone
    {
        public string DisplayName { get; set; }
        public string RefName { get; set; }
        public Vector2Int Position { get; set; }

        public Level[,] Levels { get; set; } = new Level[3, 3];

        public Zone(string displayName, string refName, Vector2Int position)
        {
            DisplayName = displayName;
            RefName = refName;
            Position = position;
        }
    }
}

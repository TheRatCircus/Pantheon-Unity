// ConnectionRule.cs
// Jerome Martina

using UnityEngine.Tilemaps;

namespace Pantheon.Gen
{
    public sealed class ConnectionRule
    {
        public string Key { get; set; } = "DEFAULT_CONNECTION_KEY";
        public int Count { get; set; } = 1;
        public Tile Tile { get; set; }
    }
}

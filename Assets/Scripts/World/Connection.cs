// Connection.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.World
{
    [System.Serializable]
    public sealed class Connection
    {
        public string Key { get; set; }
        public Vector2Int Position { get; set; }
        public Connection Partner { get; set; }
        public Tile Tile { get; set; }
    }
}

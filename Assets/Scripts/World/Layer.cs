// Layer.cs
// Jerome Martina

using Pantheon.Gen;
using UnityEngine;
using System.Collections.Generic;

namespace Pantheon.World
{
    /// <summary>
    /// A horizontal slice of the game world.
    /// </summary>
    public sealed class Layer
    {
        public int ZLevel { get; private set; }

        public Dictionary<Vector2Int, Level> Levels { get; private set; }
            = new Dictionary<Vector2Int, Level>();

        public event System.Action<Layer, Vector2Int> LevelRequestEvent;

        public Layer(int z)
        {
            ZLevel = z;

            if (ZLevel == 0)
            {
                
            }
        }

        public void RequestLevel(Vector2Int pos)
        {
            if (Levels.TryGetValue(pos, out Level level))
                ; // Do something here
            else
                LevelRequestEvent?.Invoke(this, pos);
        }
    }
}

// Layer.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// A horizontal slice of the game world.
    /// </summary>
    public sealed class Layer : MonoBehaviour
    {
        public int ZLevel { get; set; }

        public Dictionary<Vector2Int, Level> Levels { get; private set; }
            = new Dictionary<Vector2Int, Level>();

        public event Func<Vector3Int, Level> LevelRequestEvent;

        public Level RequestLevel(Vector2Int pos)
        {
            if (!Levels.ContainsKey(pos))
            {
                Level newLevel = LevelRequestEvent?.Invoke(
                    new Vector3Int(pos.x, pos.y, ZLevel));
                Levels.Add(pos, newLevel);
                return newLevel;
            }
            else
            {
                Levels.TryGetValue(pos, out Level ret);
                return ret;
            }
        }
    }
}

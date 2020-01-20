// LevelWrapper.cs
// Jerome Martina

using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public sealed class LevelWrapper : MonoBehaviour
    {
        public Level Level { get; set; }

        public void OnDrawGizmosSelected()
        {
            for (int x = 0; x < Level.FleeMap.Map.GetLength(0); x++)
                for (int y = 0; y < Level.FleeMap.Map.GetLength(1); y++)
                {
                    if (Level.FleeMap.Map[x, y] > 30 ||
                        Level.FleeMap.Map[x, y] <= -30)
                        continue;

                    // TODO: Label Dijkstra map positions
                }
        }
    }
}

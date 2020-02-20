#define DEBUG_DIJKSTRA
//#undef DEBUG_DIJKSTRA

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon
{
    public sealed class LevelWrapper : MonoBehaviour
    {
        public Level Level { get; set; }

#if DEBUG_DIJKSTRA
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
                Level.RecalculatePlayerDijkstra();
        }
#endif

        private void OnDrawGizmosSelected()
        {
            if (Level == null || Level.PlayerDijkstra == null)
                return;

            for (int y = 0; y < Level.Size.y; y++)
                for (int x = 0; x < Level.Size.x; x++)
                {
                    Color color;
                    int m = Level.PlayerDijkstra.Map[x, y];

                    if (m == 255)
                        continue;

                    switch (m)
                    {
                        case 0:
                            color = Color.white;
                            break;
                        case 1:
                            color = Color.red;
                            break;
                        case 2:
                            color = Colours._orange;
                            break;
                        case 3:
                            color = Color.yellow;
                            break;
                        case 4:
                            color = Color.green;
                            break;
                        case 5:
                            color = Color.blue;
                            break;
                        case 6:
                            color = Color.magenta;
                            break;
                        default:
                            color = Color.gray;
                            break;
                    }

                    Gizmos.color = color;
                    Gizmos.DrawCube(new Vector3(x, y), new Vector3(.1f, .1f, .1f));
                }
        }
    }
}

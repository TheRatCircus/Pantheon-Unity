// Visualisation.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Debug
{
    /// <summary>
    /// Debugging methods which use UnityEngine.Debug's draw functionality.
    /// </summary>
    public static class Visualisation
    {
        /// <summary>
        /// Draw an X on a cell (aligned to world grid).
        /// </summary>
        /// <param name="cell"></param>
        public static void MarkCell(Cell cell, float duration)
        {
            Vector3 start = cell.Position.ToVector3();
            Vector3 end = new Vector3(start.x + .2f, start.y + .2f);
            UnityEngine.Debug.DrawLine(start, end, Color.cyan, duration);
            start.y += 1f;
            end.y -= 1f;
            UnityEngine.Debug.DrawLine(start, end, Color.cyan, duration);
        }

        /// <summary>
        /// Draw an X on a position.
        /// </summary>
        /// <param name="pos"></param>
        public static void MarkPos(Vector2Int pos, Color color, float duration)
        {
            Vector3 start = pos.ToVector3();
            start.x -= .5f; // Offset to put center of x on actual position
            start.y -= .5f;
            Vector3 end = new Vector3(start.x + 1f, start.y + 1f);
            UnityEngine.Debug.DrawLine(start, end, color, duration);
            start.y += 1f;
            end.y -= 1f;
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }
    }
}
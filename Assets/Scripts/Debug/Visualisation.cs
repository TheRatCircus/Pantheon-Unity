// Visualisation.cs
// Jerome Martina

using UnityEngine;
using Pantheon.World;
using Pantheon.Utils;

namespace Pantheon.Debug
{
    /// <summary>
    /// Debugging methods which use UnityEngine.Debug's draw functionality.
    /// </summary>
    public static class Visualisation
    {
        public static void MarkCell(Cell cell)
        {
            Vector3 start = Helpers.V2IToV3(cell.Position);
            Vector3 end = new Vector3(start.x + .2f, start.y + .2f);
            UnityEngine.Debug.DrawLine(start, end, Color.cyan, 5);
        }
    }
}
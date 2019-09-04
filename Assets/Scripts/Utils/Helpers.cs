// Helpers.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Utils
{
    /// <summary>
    /// Miscellaneous helper functions which belong nowhere else.
    /// </summary>
    public static class Helpers
    {
        // Convert a Vector2Int to a Vector3
        public static Vector3 V2IToV3(Vector2Int gridPos)
            => new Vector3(gridPos.x, gridPos.y);

        // Swap two references
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}


// LevelRect.cs
// Jerome Martina
// Credit to TStand90

using System;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// An abstract rectangle in world space.
    /// </summary>
    public sealed class LevelRect
    {
        public int x1, x2, y1, y2;

        public int Width => x2 - x1;
        public int Height => y2 - y1;

        public LevelRect(Vector2Int pos, Vector2Int dims)
        {
            x1 = pos.x;
            y1 = pos.y;
            x2 = pos.x + dims.x;
            y2 = pos.y + dims.y;
        }

        public static bool IsNeighbour(LevelRect a, LevelRect b)
        {
            /// # DESCRIPTION
            /// Determine whether rectangles a and b are neighbors
            /// by projecting them onto both axes and comparing their
            /// combined projections ("one-dimensional shadows") to
            /// their actual sizes.
            /// If a projection:
            ///     - is smaller than both rectangles' width/height,
            ///     then the rectangles overlap on the x/ y - axis.
            ///     - is equivalent to both rectangles' width/height,
            ///     then the rectangles are touching on the x / y - axis.
            ///     - is greater than both rectangles' width/height,
            ///     then the rectangles can not be neighbors.
            /// 
            /// Return true iff the overlap on one axis is greater than zero
            /// while the overlap on the other axis is equal to zero.
            /// (If both overlaps were greater than zero, the rectangles
            /// would be overlapping. If both overlaps were equal to zero,
            /// the rectangles would be touching on a corner only.)

            int xProjection = Math.Max(a.x2, b.x2) - Math.Min(a.x1, b.x1);
            int xOverlap = a.Width + b.Width - xProjection;

            int yProjection = Math.Max(a.y2, b.y2) - Math.Min(a.y1, b.y1);
            int yOverlap = a.Height + b.Height - yProjection;

            return xOverlap > 0 && yOverlap == 0 ||
                xOverlap == 0 && yOverlap > 0;
        }

        public Vector2Int Center()
        {
            int centerX = (x1 + x2) / 2;
            int centerY = (y1 + y2) / 2;
            return new Vector2Int(centerX, centerY);
        }

        public bool Intersects(LevelRect other)
        {
            return (x1 <= other.x2 && x2 >= other.x1
                && y1 <= other.y2 && y2 >= other.y1);
        }

        public bool Contains(Vector2Int position)
        {
            return
                position.x >= x1 && position.y >= y1 &&
                position.x <= x2 && position.y <= y2;
        }

        public bool Contains(int x, int y)
        {
            return
                x >= x1 && y >= y1 && x <= x2 && y <= y2;
        }
    }
}

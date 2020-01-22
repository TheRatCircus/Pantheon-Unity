// Bresenham.cs
// Courtesy of Jason Morley

using UnityEngine;
using Pantheon.World;

namespace Pantheon.Utils
{
    public static class Bresenhams
    {
        // Plot a path via Bresenham's Line Algorithm
        public static Line GetLine(Level level, Cell origin, Cell target)
        {
            Line ret = new Line();

            bool Plot(int cellX, int cellY)
            {
                Cell cell = level.GetCell(new Vector2Int(cellX, cellY));
                ret.Add(cell);
                return true;
            }

            int
                x0 = origin.Position.x,
                x1 = target.Position.x,
                y0 = origin.Position.y,
                y1 = target.Position.y;

            bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
            bool reverse = false;
            if (steep)
            {
                Helpers.Swap(ref x0, ref y0);
                Helpers.Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Helpers.Swap(ref x0, ref x1);
                Helpers.Swap(ref y0, ref y1);
                reverse = true;
            }

            int
                dX = (x1 - x0),
                dY = Mathf.Abs(y1 - y0),
                err = (dX / 2),
                yStep = (y0 < y1 ? 1 : -1),
                y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (!(steep ? Plot(y, x) : Plot(x, y))) break;
                err = err - dY;
                if (err < 0) { y += yStep; err += dX; }
            }

            if (reverse)
                ret.Reverse();

            return ret;
        }
    }

}

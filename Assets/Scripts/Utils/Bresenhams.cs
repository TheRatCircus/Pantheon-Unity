// Bresenham.cs
// Courtesy of Jason Morley

using UnityEngine;
using Pantheon.World;

namespace Pantheon.Utils
{
    public static class Bresenhams
    {
        public static Line GetLine(Level level, Vector2Int origin, Vector2Int target)
        {
            Line ret = new Line();

            bool Plot(int cellX, int cellY)
            {
                Vector2Int v = new Vector2Int(cellX, cellY);
                if (level.Contains(v))
                {
                    ret.Add(v);
                    return true;
                }
                else return false;
            }

            int
                x0 = origin.x,
                x1 = target.x,
                y0 = origin.y,
                y1 = target.y;

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

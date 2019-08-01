// Credit to Jason Morley
using System.Collections.Generic;
using UnityEngine;

public static class Bresenham
{
    // Plot a path via Bresenham's Line Algorithm
    static private List<Cell> GetPath(Cell origin, Cell target)
    {
        List<Cell> ret = new List<Cell>();

        int x0 = origin.Position.x;
        int x1 = target.Position.x;
        int y0 = origin.Position.y;
        int y1 = target.Position.y;

        bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        if (steep)
        {
            Helpers.Swap(ref x0, ref y0);
            Helpers.Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Helpers.Swap(ref x0, ref x1);
            Helpers.Swap(ref y0, ref y1);
        }

        int dX = (x1 - x0), dY = Mathf.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

        for (int x = x0; x <= x1; ++x)
        {
            //if (!(steep ? Plot(y, x) : Plot(x, y))) return ret;
            err = err - dY;
            if (err < 0)
                y += ystep; err += dX;
        }

        return ret;
    }
}

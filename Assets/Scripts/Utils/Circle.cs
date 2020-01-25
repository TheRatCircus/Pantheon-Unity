// Circle.cs
// Courtesy of Shivam Pradhan

using System;

namespace Pantheon.Utils
{
    public static class Circle
    {
        /// <summary>
        /// Bresenham's circle algorithm.
        /// </summary>
        /// <param name="xc">X coordinate of circle centre.</param>
        /// <param name="yc">Y coordinate of circle centre.</param>
        /// <param name="r">Radius.</param>
        /// <param name="action"></param>
        public static void DrawCircle(int xc, int yc, int r,
            Action<int, int> action)
        {
            int x = 0, y = r;
            int d = 3 - 2 * r;
            Subsequence();
            while (y >= x)
            {
                x++;

                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                {
                    d = d + 4 * x + 6;
                }
                Subsequence();
            }

            void Subsequence()
            {
                action.Invoke(xc + x, yc + y);
                action.Invoke(xc - x, yc + y);
                action.Invoke(xc + x, yc - y);
                action.Invoke(xc - x, yc - y);
                action.Invoke(xc + y, yc + x);
                action.Invoke(xc - y, yc + x);
                action.Invoke(xc + y, yc - x);
                action.Invoke(xc - y, yc - x);
            }
        }

        /// <summary>
        /// Draw an arc from north to east.
        /// </summary>
        public static void DrawArc(int xc, int yc, int r,
            Action<int, int> action)
        {
            int x = 0, y = r;
            int d = 3 - 2 * r;
            Subsequence();
            while (y >= x)
            {
                x++;

                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                {
                    d = d + 4 * x + 6;
                }
                Subsequence();
            }

            void Subsequence()
            {
                action.Invoke(xc + x, yc + y);
                action.Invoke(xc + y, yc + x);
            }
        }
    }
}

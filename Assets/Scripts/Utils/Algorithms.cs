// Algorithms.cs
// Jerome Martina

using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Utils
{
    public static class Algorithms
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
            // Credit to Shivam Pradhan
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

        public static HashSet<Cell> FloodFill(Level level, Cell start)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);
                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }

        public static HashSet<Cell> FloodFill(Level level, LevelRect rect,
            Cell start)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier) &&
                                rect.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);
                            open.Add(frontierCell);
                        }
                    open.RemoveAt(i);
                }
            }
            return filled;
        }

        public static HashSet<Cell> FloodFill(Level level, Cell start,
            Action<Cell> onFillDelegate)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            onFillDelegate.Invoke(frontierCell);
                            filled.Add(frontierCell);
                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }

        public static HashSet<Cell> FloodFill(Level level, Cell start,
            Predicate<Cell> onFillPredicate)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked || frontierCell.Actor != null)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);

                            if (onFillPredicate.Invoke(frontierCell))
                            {
                                return filled;
                            }

                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }

        /// <summary>
        /// Flood filler which runs a fallible callback i times.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="start"></param>
        /// <param name="iterateFillDelegate"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static HashSet<Cell> FloodFill(Level level, Cell start,
            Func<Cell, bool> iterateFillDelegate, int iterations)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            int callbacksComplete = 0;

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            if (callbacksComplete == iterations)
                                return filled;

                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (iterateFillDelegate.Invoke(frontierCell))
                                callbacksComplete++;

                            filled.Add(frontierCell);
                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }
    }
}

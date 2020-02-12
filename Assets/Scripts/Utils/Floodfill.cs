// Floodfill.cs
// Jerome Martina

using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Utils
{
    public static class Floodfill
    {
        public static HashSet<Cell> FillRect(Level level, LevelRect rect,
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

                            if (frontierCell.Walled)
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

        /// <summary>
        /// Flood fill only if a considered cell meets a condition.
        /// </summary>
        /// <param name="predicate">Cell is not filled if predicate fails.</param>
        /// <returns>All cells filled.</returns>
        public static HashSet<Cell> StackFillIf(
            Level level,
            Cell origin,
            Predicate<Cell> predicate)
        {
            HashSet<Cell> ret = new HashSet<Cell>();
            Stack<Cell> cells = new Stack<Cell>();
            cells.Push(origin);

            while (cells.Count > 0)
            {
                Cell a = cells.Pop();

                if (a.X > level.Size.x || a.X < 0 ||
                    a.Y > level.Size.y || a.Y < 0)
                    continue;

                if (!predicate(a))
                    continue;

                if (ret.Contains(a))
                    continue;

                ret.Add(a);

                if (level.TryGetCell(a.X - 1, a.Y, out Cell left))
                    cells.Push(left);
                if (level.TryGetCell(a.X + 1, a.Y, out Cell right))
                    cells.Push(right);
                if (level.TryGetCell(a.X, a.Y - 1, out Cell down))
                    cells.Push(down);
                if (level.TryGetCell(a.X, a.Y + 1, out Cell up))
                    cells.Push(up);
            }
            return ret;
        }

        /// <summary>
        /// Flood fill only if a considered cell meets a condition.
        /// </summary>
        /// <param name="predicate">Cell is not filled if predicate fails.</param>
        /// <returns>All cells filled.</returns>
        public static HashSet<Cell> QueueFillIf(
            Level level, 
            Cell origin,
            Predicate<Cell> predicate)
        {
            HashSet<Cell> ret = new HashSet<Cell>();
            Queue<Cell> cells = new Queue<Cell>();
            cells.Enqueue(origin);

            while (cells.Count > 0)
            {
                Cell a = cells.Dequeue();

                if (a.X > level.Size.x || a.X < 0 ||
                    a.Y > level.Size.y || a.Y < 0)
                    continue;

                if (!predicate(a))
                    continue;

                if (ret.Contains(a))
                    continue;

                ret.Add(a);

                if (level.TryGetCell(a.X - 1, a.Y, out Cell left))
                    cells.Enqueue(left);
                if (level.TryGetCell(a.X + 1, a.Y, out Cell right))
                    cells.Enqueue(right);
                if (level.TryGetCell(a.X, a.Y - 1, out Cell down))
                    cells.Enqueue(down);
                if (level.TryGetCell(a.X, a.Y + 1, out Cell up))
                    cells.Enqueue(up);
            }

            return ret;
        }

        public static Cell QueueFillForCell(
            Level level,
            Cell origin,
            Predicate<Cell> stopPredicate,
            Predicate<Cell> returnPredicate)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            Queue<Cell> cells = new Queue<Cell>();
            cells.Enqueue(origin);

            while (cells.Count > 0)
            {
                Cell a = cells.Dequeue();
                Debug.Visualisation.MarkCell(a, 10f);


                if (a.X > level.Size.x || a.X < 0 ||
                    a.Y > level.Size.y || a.Y < 0)
                    continue;

                if (returnPredicate(a))
                    return a;

                if (!stopPredicate(a))
                    continue;

                if (filled.Contains(a))
                    continue;

                filled.Add(a);

                if (level.TryGetCell(a.X - 1, a.Y, out Cell left))
                    cells.Enqueue(left);
                if (level.TryGetCell(a.X + 1, a.Y, out Cell right))
                    cells.Enqueue(right);
                if (level.TryGetCell(a.X, a.Y - 1, out Cell down))
                    cells.Enqueue(down);
                if (level.TryGetCell(a.X, a.Y + 1, out Cell up))
                    cells.Enqueue(up);
            }

            return null;
        }
    }
}

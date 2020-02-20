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
        public static HashSet<Vector2Int> FillRect(
            Level level, 
            LevelRect rect,
            Vector2Int start)
        {
            HashSet<Vector2Int> filled = new HashSet<Vector2Int>();
            List<Vector2Int> open = new List<Vector2Int>();
            HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

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
                            Vector2Int frontier = open[i];
                            frontier += new Vector2Int(x, y);

                            if (!level.Contains(frontier) ||
                                !rect.Contains(frontier))
                                continue;

                            if (closed.Contains(frontier))
                                continue;

                            if (level.Walled(frontier))
                            {
                                closed.Add(frontier);
                                continue;
                            }

                            if (filled.Contains(frontier))
                            {
                                closed.Add(frontier);
                                continue;
                            }

                            filled.Add(frontier);
                            open.Add(frontier);
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
        public static List<Vector2Int> StackFillIf(
            Level level,
            Vector2Int origin,
            Predicate<Vector2Int> predicate)
        {
            bool[,] filled = new bool[level.Size.x, level.Size.y];
            List<Vector2Int> ret = new List<Vector2Int>();
            Stack<Vector2Int> cells = new Stack<Vector2Int>();
            cells.Push(origin);

            while (cells.Count > 0)
            {
                Vector2Int a = cells.Pop();

                if (filled[a.x, a.y])
                    continue;

                if (!predicate(a))
                    continue;

                if (!level.Contains(a))
                    continue;

                ret.Add(a);
                filled[a.x, a.y] = true;

                Vector2Int left = new Vector2Int(a.x - 1, a.y);
                if (level.Contains(left))
                    cells.Push(left);
                Vector2Int right = new Vector2Int(a.x + 1, a.y);
                if (level.Contains(right))
                    cells.Push(right);
                Vector2Int down = new Vector2Int(a.x, a.y - 1);
                if (level.Contains(down))
                    cells.Push(down);
                Vector2Int up = new Vector2Int(a.x, a.y + 1);
                if (level.Contains(up))
                    cells.Push(up);
            }
            return ret;
        }

        /// <summary>
        /// Flood fill only if a considered cell meets a condition.
        /// </summary>
        /// <param name="predicate">Cell is not filled if predicate fails.</param>
        /// <returns>All cells filled.</returns>
        public static HashSet<Vector2Int> QueueFillIf(
            Level level, 
            Vector2Int origin,
            Predicate<Vector2Int> predicate)
        {
            HashSet<Vector2Int> ret = new HashSet<Vector2Int>();
            Queue<Vector2Int> cells = new Queue<Vector2Int>();
            cells.Enqueue(origin);

            while (cells.Count > 0)
            {
                Vector2Int a = cells.Dequeue();

                if (!level.Contains(a))
                    continue;

                if (!predicate(a))
                    continue;

                if (ret.Contains(a))
                    continue;

                ret.Add(a);

                Vector2Int left = new Vector2Int(a.x - 1, a.y);
                if (level.Contains(left))
                    cells.Enqueue(left);
                Vector2Int right = new Vector2Int(a.x + 1, a.y);
                if (level.Contains(right))
                    cells.Enqueue(right);
                Vector2Int down = new Vector2Int(a.x, a.y - 1);
                if (level.Contains(down))
                    cells.Enqueue(down);
                Vector2Int up = new Vector2Int(a.x, a.y + 1);
                if (level.Contains(up))
                    cells.Enqueue(up);
            }

            return ret;
        }

        /// <summary>
        /// Flood fill until meeting a cell that passes a condition.
        /// </summary>
        /// <param name="continuePredicate">Do not fill cell if this passes.</param>
        /// <param name="returnPredicate">Return candidate immediately if this passes.</param>
        public static Vector2Int QueueFillForCell(
            Level level,
            Vector2Int origin,
            Predicate<Vector2Int> continuePredicate,
            Predicate<Vector2Int> returnPredicate)
        {
            HashSet<Vector2Int> filled = new HashSet<Vector2Int>();
            Queue<Vector2Int> cells = new Queue<Vector2Int>();
            cells.Enqueue(origin);

            while (cells.Count > 0)
            {
                Vector2Int a = cells.Dequeue();

                if (!level.Contains(a))
                    continue;

                if (returnPredicate(a))
                    return a;

                if (continuePredicate(a))
                    continue;

                if (filled.Contains(a))
                    continue;

                filled.Add(a);

                Vector2Int left = new Vector2Int(a.x - 1, a.y);
                if (level.Contains(left))
                    cells.Enqueue(left);
                Vector2Int right = new Vector2Int(a.x + 1, a.y);
                if (level.Contains(right))
                    cells.Enqueue(right);
                Vector2Int down = new Vector2Int(a.x, a.y - 1);
                if (level.Contains(down))
                    cells.Enqueue(down);
                Vector2Int up = new Vector2Int(a.x, a.y + 1);
                if (level.Contains(up))
                    cells.Enqueue(up);
            }

            return Level.NullCell;
        }

        public static HashSet<Vector2Int> QueueFillToCapacity(
            Level level,
            Vector2Int origin,
            int capacity)
        {
            HashSet<Vector2Int> ret = new HashSet<Vector2Int>();
            Queue<Vector2Int> cells = new Queue<Vector2Int>();
            cells.Enqueue(origin);

            while (cells.Count > 0 && ret.Count < capacity)
            {
                Vector2Int a = cells.Dequeue();

                if (!level.Contains(a))
                    continue;

                if (ret.Contains(a))
                    continue;

                ret.Add(a);

                Vector2Int left = new Vector2Int(a.x - 1, a.y);
                if (level.Contains(left))
                    cells.Enqueue(left);
                Vector2Int right = new Vector2Int(a.x + 1, a.y);
                if (level.Contains(right))
                    cells.Enqueue(right);
                Vector2Int down = new Vector2Int(a.x, a.y - 1);
                if (level.Contains(down))
                    cells.Enqueue(down);
                Vector2Int up = new Vector2Int(a.x, a.y + 1);
                if (level.Contains(up))
                    cells.Enqueue(up);
            }

            return ret;
        }
    }
}

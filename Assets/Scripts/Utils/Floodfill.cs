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

                            if (frontierCell.Wall != null)
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

        public static HashSet<Cell> FillLevel(
            Level level,
            Cell origin,
            Predicate<Cell> condition)
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

                if (ret.Contains(a))
                    continue;

                if (!condition(a))
                    continue;

                ret.Add(a);
                cells.Push(level.GetCell(a.X - 1, a.Y));
                cells.Push(level.GetCell(a.X + 1, a.Y));
                cells.Push(level.GetCell(a.X, a.Y - 1));
                cells.Push(level.GetCell(a.X, a.Y + 1));
            }
            return ret;
        }
    }
}

// Floodfill.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Utils
{
    public static class Floodfill
    {
        public static HashSet<Vector2Int> FillRect(
            Level level, LevelRect rect, Vector2Int start)
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

                            if (level.CellIsWalled(frontier))
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
    }
}

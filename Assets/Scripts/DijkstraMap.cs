// DijkstraMap.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// General-purpose Dijkstra map to lay on a level.
    /// </summary>
    public sealed class DijkstraMap
    {
        public Level Level { get; private set; }

        public Dictionary<Vector2Int, int> Map { get; private set; }
            = new Dictionary<Vector2Int, int>();
        private List<Vector2Int> open = new List<Vector2Int>();
        private HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

        public DijkstraMap(Level level) => Level = level;

        public void Recalculate(IEnumerable<Cell> goals)
        {
            Map.Clear();
            open.Clear();
            closed.Clear();

            foreach (Cell c in goals)
            {
                Map.Add(c.Position, 0);
                open.Add(c.Position);
            }

            int iterations = 0; // Arbitrary limiter
            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Vector2Int> temp = new List<Vector2Int>();
                for (int i = 0; i < open.Count; i++)
                {
                    Map.TryGetValue(open[i], out int prevDist);
                    closed.Add(open[i]); // Immediately close origin

                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = new Vector2Int(open[i].x + x,
                                open[i].y + y);

                            if (closed.Contains(frontier))
                                continue;

                            if (!Level.Contains(frontier) || 
                                Level.GetCell(frontier).Blocked ||
                                Level.GetCell(frontier).Actor != null)
                            {
                                closed.Add(frontier);
                                continue;
                            }

                            if (Map.ContainsKey(frontier))
                            {
                                closed.Add(frontier);
                                continue;
                            }

                            Map.Add(frontier, prevDist + 1);
                            temp.Add(frontier);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
                iterations++;
            }
        }

        public Vector2Int RollDownhill(Cell origin)
        {
            Vector2Int lowestPosition = Vector2Int.zero;
            int lowest = 255;

            for (int x = origin.Position.x - 1; x <= origin.Position.x + 1;
                x++)
                for (int y = origin.Position.y - 1; y <= origin.Position.y + 1;
                    y++)
                {
                    if (x == origin.Position.x && y == origin.Position.y)
                        continue;

                    if (!Map.TryGetValue(new Vector2Int(x, y), out int weight))
                        continue;

                    if (weight < lowest)
                    {
                        lowest = weight;
                        lowestPosition = new Vector2Int(x, y);
                    }
                }
            return lowestPosition;
        }
    }
}

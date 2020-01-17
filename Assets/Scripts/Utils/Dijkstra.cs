// DijkstraMap.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon.Utils
{
    /// <summary>
    /// General-purpose Dijkstra map to lay on a level.
    /// </summary>
    public sealed class DijkstraMap
    {
        private Level level;

        public int[,] Map { get; private set; }
        private HashSet<Cell> goals = new HashSet<Cell>();
        private List<Vector2Int> open = new List<Vector2Int>();
        private HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

        public DijkstraMap(Level level)
        {
            this.level = level;
            Map = new int[level.CellSize.x, level.CellSize.y];
        }

        public void SetGoals(IEnumerable<Cell> goals)
        {
            this.goals = (HashSet<Cell>)goals;
        }

        public void SetGoals(params Cell[] goals)
        {
            this.goals.Clear();
            foreach (Cell c in goals)
            {
                this.goals.Add(c);
            }
        }

        public void Recalculate()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y] = 255;
                }

            foreach (Cell c in goals)
            {
                Map[c.Position.x, c.Position.y] = 0;
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
                    int prevDist = Map[open[i].x, open[i].y];
                    closed.Add(open[i]); // Immediately close origin

                    Profiler.BeginSample("Dijkstra Map Flood Fill");
                    int x = -1;
                    for (; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = new Vector2Int(open[i].x + x,
                                open[i].y + y);

                            if (closed.Contains(frontier))
                                continue;

                            if (!level.Contains(frontier) ||
                                level.CellTerrainIsBlocked(frontier) ||
                                level.CellHasActor(frontier))
                            {
                                closed.Add(frontier);
                                continue;
                            }

                            if (Map[frontier.x, frontier.y] < 255)
                            {
                                closed.Add(frontier);
                                continue;
                            }

                            Map[frontier.x, frontier.y] = prevDist + 1;
                            temp.Add(frontier);
                        }

                    Profiler.EndSample();
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
                iterations++;
            }
            closed.Clear();
            
        }

        public System.Collections.IEnumerator RecalculateAsync()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y] = 255;
                }

            foreach (Cell c in goals)
            {
                Map[c.Position.x, c.Position.y] = 0;
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
                    int prevDist = Map[open[i].x, open[i].y];
                    closed.Add(open[i]); // Immediately close origin

                    int x = -1;
                    for (; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            yield return new WaitForSeconds(.1f);

                            Vector2Int frontier = new Vector2Int(open[i].x + x,
                                open[i].y + y);

                            if (closed.Contains(frontier))
                                continue;

                            if (!level.Contains(frontier) ||
                                level.CellTerrainIsBlocked(frontier) ||
                                level.CellHasActor(frontier))
                            {
                                closed.Add(frontier);
                                continue;
                            }

                            Map[frontier.x, frontier.y] = prevDist + 1;
                            temp.Add(frontier);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
                iterations++;
            }
            closed.Clear();
        }

        public Vector2Int RollDownhill(Cell origin)
        {
            Vector2Int lowestPosition = origin.Position;
            int lowest = 255;

            for (int x = origin.Position.x - 1; x <= origin.Position.x + 1;
                x++)
                for (int y = origin.Position.y - 1; y <= origin.Position.y + 1;
                    y++)
                {
                    if (x == origin.Position.x && y == origin.Position.y)
                        continue;

                    if (!Map.TryGet(out int weight, x, y))
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

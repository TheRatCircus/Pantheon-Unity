// DijkstraMap.cs
// Jerome Martina

using Pantheon.World;
using System;
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
        private readonly Level level;

        public int[,] Map { get; private set; }
        private HashSet<Vector2Int> goals = new HashSet<Vector2Int>();
        private List<Vector2Int> open = new List<Vector2Int>();
        private HashSet<Vector2Int> temp = new HashSet<Vector2Int>();

        public DijkstraMap(Level level)
        {
            this.level = level;
            Map = new int[level.CellSize.x, level.CellSize.y];
        }

        public void SetGoals(IEnumerable<Vector2Int> goals)
        {
            this.goals = (HashSet<Vector2Int>)goals;
        }

        public void SetGoals(params Vector2Int[] goals)
        {
            this.goals.Clear();
            foreach (Vector2Int c in goals)
                this.goals.Add(c);
        }

        public void Recalculate(Func<Level, Vector2Int, bool> predicate)
        {
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y] = 255;
                }

            foreach (Vector2Int c in goals)
            {
                Map[c.x, c.y] = 0;
                open.Add(c);
            }
            
            while (open.Count > 0)
            {
                for (int i = 0; i < open.Count; i++)
                {
                    int prevDist = Map[open[i].x, open[i].y];

                    Profiler.BeginSample("Dijkstra Map Flood Fill");
                    int x = -1;
                    for (; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = new Vector2Int(open[i].x + x,
                                open[i].y + y);

                            if (!level.Contains(frontier) ||
                                level.CellIsWalled(frontier) ||
                                level.CellHasActor(frontier))
                                continue;

                            if (!predicate(level, frontier))
                                continue;

                            if (Map[frontier.x, frontier.y] < 255)
                                continue;

                            Map[frontier.x, frontier.y] = prevDist + 1;
                            temp.Add(frontier);
                        }

                    Profiler.EndSample();
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
        }

        public System.Collections.IEnumerator RecalculateAsync()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y] = 255;
                }

            foreach (Vector2Int c in goals)
            {
                Map[c.x, c.y] = 0;
                open.Add(c);
            }

            while (open.Count > 0)
            {
                for (int i = 0; i < open.Count; i++)
                {
                    int prevDist = Map[open[i].x, open[i].y];
                    int x = -1;

                    for (; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            yield return null;

                            Vector2Int frontier = new Vector2Int(open[i].x + x,
                                open[i].y + y);

                            if (!level.Contains(frontier) ||
                                level.CellIsWalled(frontier) ||
                                level.CellHasActor(frontier))
                                continue;


                            if (Map[frontier.x, frontier.y] < 255)
                                continue;

                            Map[frontier.x, frontier.y] = prevDist + 1;
                            temp.Add(frontier);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
        }

        public void Invert()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y] = Mathf.RoundToInt(Map[x, y] * -1.2f);
                }
        }

        public Vector2Int RollDownhill(Vector2Int origin)
        {
            Vector2Int lowestPosition = origin;
            int lowest = 255;

            for (int x = origin.x - 1; x <= origin.x + 1;
                x++)
                for (int y = origin.y - 1; y <= origin.y + 1;
                    y++)
                {
                    if (x == origin.x && y == origin.y)
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

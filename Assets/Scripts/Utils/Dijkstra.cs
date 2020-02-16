// DijkstraMap.cs
// Jerome Martina

using Pantheon.World;
using System;
using System.Collections;
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
        private HashSet<Vector2Int> goals = new HashSet<Vector2Int>();
        private Queue<Vector2Int> queue = new Queue<Vector2Int>();
        private HashSet<Vector2Int> modified = new HashSet<Vector2Int>();

        public DijkstraMap(Level level)
        {
            this.level = level;
            Map = new int[level.Size.x, level.Size.y];
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

        public void Recalculate(Predicate<Vector2Int> predicate)
        {
            Profiler.BeginSample("Dijkstra Map Recalc");

            Profiler.BeginSample("Dijkstra Map: Clear");
            if (modified.Count < 1)
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                    for (int y = 0; y < Map.GetLength(1); y++)
                    {
                        Map[x, y] = 255;
                    }
            }
            else
            {
                foreach (Vector2Int c in modified)
                    Map[c.x, c.y] = 255;
                modified.Clear();
            }
            Profiler.EndSample();

            foreach (Vector2Int c in goals)
            {
                Map[c.x, c.y] = 0;
                queue.Enqueue(c);
            }

            while (queue.Count > 0)
            {
                Vector2Int a = queue.Dequeue();

                if (Map[a.x, a.y] > 0 && Map[a.x, a.y] < 255) // Already covered
                    continue;

                if (!predicate(a))
                    continue;

                if (level.Walled(a.x, a.y))
                    continue;

                int lowestAdjacent = Map[a.x, a.y];

                for (int y = -1; y <= 1; y++)
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        if (!level.Contains(a.x + x, a.y + y))
                            continue;

                        if (Map[a.x + x, a.y + y] < lowestAdjacent)
                            lowestAdjacent = Map[a.x + x, a.y + y];
                    }
                                
                Map[a.x, a.y] = lowestAdjacent + 1;
                modified.Add(a);

                Vector2Int left = new Vector2Int(a.x - 1, a.y);
                if (level.Contains(left))
                    queue.Enqueue(left);
                Vector2Int right = new Vector2Int(a.x + 1, a.y);
                if (level.Contains(right))
                    queue.Enqueue(right);
                Vector2Int down = new Vector2Int(a.x, a.y - 1);
                if (level.Contains(down))
                    queue.Enqueue(down);
                Vector2Int up = new Vector2Int(a.x, a.y + 1);
                if (level.Contains(up))
                    queue.Enqueue(up);
            }

            Profiler.EndSample();
        }

        public IEnumerator RecalculateDebug(Predicate<Vector2Int> predicate)
        {
            for (int x = 0; x < Map.GetLength(0); x++)
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y] = 255;
                }

            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            foreach (Vector2Int c in goals)
            {
                Map[c.x, c.y] = 0;
                queue.Enqueue(c);
            }

            while (queue.Count > 0)
            {
                yield return new WaitForSeconds(.01f);
                Vector2Int a = queue.Dequeue();

                if (Map[a.x, a.y] > 0 && Map[a.x, a.y] < 255) // Already covered
                    continue;

                if (!predicate(a))
                    continue;

                if (level.Walled(a.x, a.y))
                    continue;

                int lowestAdjacent = Map[a.x, a.y];

                for (int y = -1; y <= 1; y++)
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        if (!level.Contains(a.x + x, a.y + y))
                            continue;

                        if (Map[a.x + x, a.y + y] < lowestAdjacent)
                            lowestAdjacent = Map[a.x + x, a.y + y];
                    }

                Map[a.x, a.y] = lowestAdjacent + 1;
                UnityEngine.Debug.Log($"{a} = {lowestAdjacent + 1}");

                Vector2Int left = new Vector2Int(a.x - 1, a.y);
                if (level.Contains(left))
                    queue.Enqueue(left);
                Vector2Int right = new Vector2Int(a.x + 1, a.y);
                if (level.Contains(right))
                    queue.Enqueue(right);
                Vector2Int down = new Vector2Int(a.x, a.y - 1);
                if (level.Contains(down))
                    queue.Enqueue(down);
                Vector2Int up = new Vector2Int(a.x, a.y + 1);
                if (level.Contains(up))
                    queue.Enqueue(up);
            }

            UnityEngine.Debug.Log($"Djikstra recalc done.");
        }

        public Vector2Int RollDownhill(Vector2Int origin)
        {
            Vector2Int lowestPosition = origin;
            int lowest = Map[origin.x, origin.y];

            for (int x = origin.x - 1; x <= origin.x + 1; x++)
                for (int y = origin.y - 1; y <= origin.y + 1; y++)
                {
                    if (x == origin.x && y == origin.y)
                        continue;

                    if (!Map.TryGet(out int weight, x, y))
                        continue;

                    if (weight <= lowest)
                    {
                        lowest = weight;
                        lowestPosition = new Vector2Int(x, y);
                    }
                }

            if (lowest == 255)
                return origin;

            return lowestPosition;
        }
    }
}

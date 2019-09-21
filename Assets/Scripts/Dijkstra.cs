// Dijkstra.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.World;

/// <summary>
/// General-purpose Dijkstra map to lay on a level.
/// </summary>
public sealed class Dijkstra
{
    private Level level;

    private Dictionary<Vector2Int, int> map = new Dictionary<Vector2Int, int>();
    private List<Vector2Int> open = new List<Vector2Int>();
    private HashSet<Vector2Int> closed = new HashSet<Vector2Int>();

    public Dijkstra(Level level) => this.level = level;

    public void Recalculate(Vector2Int[] goals)
    {
        map.Clear();
        open.Clear();
        closed.Clear();

        Debug.Log("Recalculating Dijkstra map...");

        foreach (Vector2Int v in goals)
        {
            map.Add(v, 0);
            open.Add(v);
        }

        int iterations = 0;
        while (open.Count > 0)
        {
            for (int i = 0; i < open.Count; i++)
            {
                map.TryGetValue(open[i], out int distance);
                closed.Add(open[i]);
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int frontier = new Vector2Int(open[i].x + x, open[i].y + y);

                        if (closed.Contains(frontier))
                            continue;

                        if (level.GetCell(frontier).Blocked)
                        {
                            closed.Add(frontier);
                            continue;
                        }
                            
                        if (map.ContainsKey(frontier))
                        {
                            closed.Add(frontier);
                            continue;
                        }

                        map.Add(frontier, distance + 1);
                        open.Add(frontier);
                    }
                open.RemoveAt(i);
            }
            iterations++;
        }
    }
}

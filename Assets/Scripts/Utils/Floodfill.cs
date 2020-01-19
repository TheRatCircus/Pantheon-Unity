// FloodFill.cs
// Jerome Martina
// with credit to Karim Oumghar

using Pantheon.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Utils
{
    public static class FloodFill
    {
        public static HashSet<Vector2Int> FillRect(
            Level level, LevelRect rect, Vector2Int origin)
        {
            HashSet<Vector2Int> ret = new HashSet<Vector2Int>();
            Stack<Vector2Int> points = new Stack<Vector2Int>();
            points.Push(origin);

            while (points.Count > 0)
            {
                Vector2Int a = points.Pop();

                if (a.x > rect.Width || a.y > rect.Height ||
                    a.x > level.CellSize.x || a.x < 0 ||
                    a.y > level.CellSize.y || a.y < 0)
                    continue;

                if (ret.Contains(a))
                    continue;

                ret.Add(a);
                points.Push(new Vector2Int(a.x - 1, a.y));
                points.Push(new Vector2Int(a.x + 1, a.y));
                points.Push(new Vector2Int(a.x, a.y - 1));
                points.Push(new Vector2Int(a.x, a.y + 1));
            }
            return ret;
        }

        public static HashSet<Vector2Int> FillLevel(
            Level level,
            Vector2Int origin,
            Func<Level, Vector2Int, bool> condition)
        {
            HashSet<Vector2Int> ret = new HashSet<Vector2Int>();
            Stack<Vector2Int> points = new Stack<Vector2Int>();
            points.Push(origin);

            while (points.Count > 0)
            {
                Vector2Int a = points.Pop();

                if (a.x > level.CellSize.x || a.x < 0 ||
                    a.y > level.CellSize.y || a.y < 0)
                    continue;

                if (ret.Contains(a))
                    continue;

                if (!condition(level, a))
                    continue;

                ret.Add(a);
                points.Push(new Vector2Int(a.x - 1, a.y));
                points.Push(new Vector2Int(a.x + 1, a.y));
                points.Push(new Vector2Int(a.x, a.y - 1));
                points.Push(new Vector2Int(a.x, a.y + 1));
            }
            return ret;
        }

        public static IEnumerator FillLevelAsync(
            Level level, Vector2Int origin)
        {
            HashSet<Vector2Int> closed = new HashSet<Vector2Int>();
            Stack<Vector2Int> points = new Stack<Vector2Int>();
            points.Push(origin);

            while (points.Count > 0)
            {
                if (points.Count > level.CellCount)
                    throw new Exception(
                        "Flood filler has exceeded number of cells in level.");

                Vector2Int a = points.Pop();
                Debug.Visualisation.MarkPos(a, Color.cyan, 10f);
                yield return new WaitForSeconds(.1f);

                if (a.x > level.CellSize.x || a.x < 0 ||
                    a.y > level.CellSize.y || a.y < 0)
                    continue;

                if (closed.Contains(a))
                    continue;

                closed.Add(a);
                points.Push(new Vector2Int(a.x - 1, a.y));
                points.Push(new Vector2Int(a.x + 1, a.y));
                points.Push(new Vector2Int(a.x, a.y - 1));
                points.Push(new Vector2Int(a.x, a.y + 1));
            }
        }
    }
}

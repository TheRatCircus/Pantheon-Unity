// Line.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Utils
{
    /// <summary>
    /// Ordered List of cells.
    /// </summary>
    [System.Serializable]
    public class Line : List<Vector2Int>
    {
        public Line() { }

        public Line(int capacity) => Capacity = capacity;

        public Line(Line line)
        {
            foreach (Vector2Int v in line) Add(v);
        }
    }
}

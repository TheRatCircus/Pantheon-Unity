// Line.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Utils
{
    /// <summary>
    /// One-dimensional ordered List of cells. Also use for paths.
    /// </summary>
    public class Line : List<Vector2Int>
    {
        public Level Level { get; set; }

        public Line() { }

        public Line(int capacity) => Capacity = capacity;

        public Line(Line line)
        {
            foreach (Vector2Int cell in line)
                Add(cell);
        }

        public bool IsObstructed()
        {
            foreach (Vector2Int cell in this)
                if (Level.CellIsBlocked(cell))
                    return true;
            
            return false;
        }
    }
}

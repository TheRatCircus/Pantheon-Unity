// Line.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Utils
{
    /// <summary>
    /// Ordered List of cells.
    /// </summary>
    public class Line : List<Cell>
    {
        public bool IsObstructed()
        {
            foreach (Cell cell in this)
            {
                if (cell.Blocked)
                    return true;
            }
            return false;
        }
    }
}

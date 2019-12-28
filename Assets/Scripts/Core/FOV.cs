// FOV.cs
// Courtesy of Bob Nystrom

using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Core
{
    public static class FOV
    {
        // Not in terms of cells
        public const int FOVRadius = 15;

        private static Cell prev;

        /// <summary>
        /// Change visibility and reveal new cells.
        /// <param name="level"></param>
        /// </summary>
        /// <returns>A HashSet of all cells affected by the refresh.</returns>
        public static HashSet<Cell> RefreshFOV(Level level, Cell origin,
            bool drawChanges)
        {
            // Hide cells at the last refresh position
            if (prev != null)
            {
                List<Cell> cells = level.GetSquare(prev, FOVRadius);
                foreach (Cell c in cells)
                    c.Visible = false;
                level.Draw(cells);
            }

            HashSet<Cell> allRefreshed = new HashSet<Cell>();
            for (int octant = 0; octant < 8; octant++)
            {
                List<Cell> refreshed = ShadowOctant(level,
                    origin.Position, octant);

                allRefreshed.AddMany(refreshed);
            }

            if (drawChanges)
                level.Draw(allRefreshed);

            prev = origin;

            return allRefreshed;
        }

        // Coordinates used to transform a point in an octant
        private static readonly Vector2Int[,] _octantCoordinates
            = new Vector2Int[,]
        {
            { new Vector2Int(0, -1), new Vector2Int(1, 0) },
            { new Vector2Int(1, 0), new Vector2Int(0, -1) },
            { new Vector2Int(1, 0), new Vector2Int(0, 1) },
            { new Vector2Int(0, 1), new Vector2Int(1, 0) },
            { new Vector2Int(0, 1), new Vector2Int(-1, 0) },
            { new Vector2Int(-1, 0), new Vector2Int(0, 1) },
            { new Vector2Int(-1, 0), new Vector2Int(0, -1) },
            { new Vector2Int(0, -1), new Vector2Int(-1, 0) }
        };

        // Generate an octant of shadows, and return the FOV area to be redrawn
        private static List<Cell> ShadowOctant(Level level, Vector2Int origin,
            int octant)
        {
            // Increments based off of octantCoordinates
            var rowInc = _octantCoordinates[octant, 0];
            var colInc = _octantCoordinates[octant, 1];

            ShadowLine line = new ShadowLine();
            bool fullShadow = false;
            List<Cell> ret = new List<Cell>();

            for (int row = 0; row < FOVRadius; row++)
            {
                // Record position
                Vector2Int pos = origin + (rowInc * row);
                // Stop once going out of bounds
                if (!level.Contains(pos)) break;

                bool pastMaxDistance = false;
                for (int col = 0; col <= row; col++)
                {
                    // Break on this row if going out of bounds
                    if (!level.Contains(pos)) break;
                    // Add new cells to list of updated cells
                    ret.Add(level.GetCell(pos));
                    // Visibility fall off over distance
                    int fallOff = 255;

                    // If entire row is known to be in shadow, set this cell to
                    // be in shadow
                    if (fullShadow || pastMaxDistance)
                        level.GetCell(pos).SetVisibility(false, fallOff);
                    else
                    {
                        fallOff = 0;
                        float distance = Vector2.Distance(origin, pos);
                        if (distance > FOVRadius)
                        {
                            fallOff = 255;
                            pastMaxDistance = true;
                        }
                        else
                        {
                            float normalized = distance /
                                FOVRadius;
                            normalized = Mathf.Pow(normalized, 2);
                            fallOff = (int)(normalized * 255);
                        }
                        Shadow projection = ProjectTile(row, col);

                        // Set the visibility of this tile
                        bool visible = !line.IsInShadow(projection);
                        level.GetCell(pos).SetVisibility(visible,
                            fallOff);

                        // Add any opaque tiles to the shadow map
                        if (visible && level.GetCell(pos).Opaque)
                        {
                            line.AddShadow(projection);
                            fullShadow = line.IsFullShadow();
                        }
                    }
                    pos += colInc;
                    if (!level.Contains(pos)) break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Creates a Shadow that corresponds to the projected silhouette of
        /// the tile at row, col.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private static Shadow ProjectTile(float row, float col)
        {
            float rowF = row;
            float colF = col;

            float topLeft = colF / (rowF + 2);
            float bottomRight = (colF + 1) / (rowF + 1);

            return new Shadow(topLeft, bottomRight,
                new Vector2(col, row + 2),
                new Vector2(col + 1, row + 1));
        }
    }

    // Generate a line of shadows
    sealed class ShadowLine
    {
        public readonly List<Shadow> Shadows = new List<Shadow>();

        // Is this projection within shadow?
        public bool IsInShadow(Shadow projection)
        {
            foreach (Shadow shadow in Shadows)
                if (shadow.Contains(projection))
                    return true;

            return false;
        }

        // Is this line in full shadow?
        public bool IsFullShadow()
        {
            return
                Shadows.Count == 1 &&
                Shadows[0].Start == 0 &&
                Shadows[0].End == 1;
        }

        // Determine where to add a new shadow to the list
        public void AddShadow(Shadow shadow)
        {
            int index = 0;

            for (; index < Shadows.Count; index++)
                // Stop when hitting the insertion point
                if (Shadows[index].Start >= shadow.Start) break;

            // Check if new shadow overlaps previous or next
            Shadow overlappingPrevious = null;
            if (index > 0 && Shadows[index - 1].End > shadow.Start)
                overlappingPrevious = Shadows[index - 1];

            Shadow overlappingNext = null;
            if (index < Shadows.Count && Shadows[index].Start < shadow.End)
                overlappingNext = Shadows[index];

            // Insert and unify with overlapping shadows
            if (overlappingNext != null)
            {
                if (overlappingPrevious != null)
                {
                    // Overlaps both, so unify one and delete the other
                    overlappingPrevious.End = overlappingNext.End;
                    overlappingPrevious.EndPos = overlappingNext.EndPos;
                    Shadows.RemoveAt(index);
                }
                else
                {
                    // Only overlaps the next shadow, so unify it with that
                    overlappingNext.Start = shadow.Start;
                    overlappingNext.StartPos = shadow.StartPos;
                }
            }
            else
            {
                if (overlappingPrevious != null)
                {
                    // Overlaps the previous one, so unify it with that
                    overlappingPrevious.End = shadow.End;
                    overlappingPrevious.EndPos = shadow.EndPos;
                }
                else
                {
                    // Does not overlap with anything, so insert
                    Shadows.Insert(index, shadow);
                }
            }
        }
    }

    /// <summary>
    /// Represents the 1D projection of a 2D shadow onto a normalized line.
    /// In other words, a range from 0.0 to 1.0.
    /// </summary>
    sealed class Shadow
    {
        public float Start { get; set; }
        public float End { get; set; }
        public Vector2 EndPos { get; set; }
        public Vector2 StartPos { get; set; }

        public Shadow(float start, float end, Vector2 startPos, Vector2 endPos)
        {
            Start = start;
            End = end;
            StartPos = startPos;
            EndPos = endPos;
        }

        public override string ToString() => $"{Start}-{End}";

        public bool Contains(Shadow other)
            => Start <= other.Start && End >= other.End;
    }
}
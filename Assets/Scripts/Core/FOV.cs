// FOV.cs
// Courtesy of Bob Nystrom

#define DEBUG_FOV
#undef DEBUG_FOV

using Pantheon.Utils;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon.Core
{
    public static class FOV
    {
        // In cells, but does not account for light falloff
        public const int Radius = 15;

        private static Vector2Int prev = Level.NullCell;

        /// <summary>
        /// Change visibility and reveal new cells.
        /// </summary>
        /// <returns>A HashSet of all cells affected by the refresh.</returns>
        public static HashSet<Vector2Int> RefreshFOV(Level level, Vector2Int origin,
            bool drawChanges)
        {
            Profiler.BeginSample("FOV: Hide Previous");
            // Hide cells at the last refresh position
            if (prev != Level.NullCell && Helpers.Distance(prev, origin) > 1)
            {
                List<Vector2Int> cells = level.GetSquare(prev, Radius);
                foreach (Vector2Int c in cells)
                {
                    if (level.SetVisibility(c.x, c.y, false, 255))
                        level.DrawTile(c);
                }
            }
            Profiler.EndSample();

            HashSet<Vector2Int> changed = new HashSet<Vector2Int>();
            for (int octant = 0; octant < 8; octant++)
            {
                Profiler.BeginSample("FOV: Octants");
                List<Vector2Int> refreshed = ShadowOctant(level, origin, octant);
                Profiler.EndSample();
                changed.AddMany(refreshed);
            }

            if (drawChanges)
                level.Draw(changed);

            prev = origin;

            Profiler.BeginSample("FOV: Get Visibles");
            HashSet<Vector2Int> visibles = Floodfill.StackFillIf(
                level, origin, 
                (Vector2Int c) => level.Visible(c.x, c.y));
            Locator.Player.UpdateVisibles(visibles);
            Profiler.EndSample();

#if DEBUG_FOV
            foreach (Vector2Int v in changed)
                DebugFOV(level, v);
#endif

            return changed;
        }

        public static void ResetPrevious() => prev = Level.NullCell;

        // Coordinates used to transform a point in an octant
        private static readonly Vector2Int[,] _octantCoords
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
        private static List<Vector2Int> ShadowOctant(Level level, Vector2Int origin,
            int octant)
        {
            // Increments based off of octantCoordinates
            var rowInc = _octantCoords[octant, 0];
            var colInc = _octantCoords[octant, 1];

            ShadowLine line = new ShadowLine();
            bool fullShadow = false;
            List<Vector2Int> ret = new List<Vector2Int>();

            for (int row = 0; row < Radius; row++)
            {
                // Record position
                Vector2Int pos = origin + (rowInc * row);
                // Stop once going out of bounds
                if (!level.Contains(pos)) break;

                bool pastMaxDistance = false;
                for (int col = 0; col <= row; col++)
                {
                    // Visibility fall off over distance
                    int fallOff = 255;
                    // If entire row is known to be in shadow,
                    // set this cell be in shadow
                    if (fullShadow || pastMaxDistance)
                    {
                        if (level.SetVisibility(pos.x, pos.y, false, fallOff))
                            ret.Add(pos);
                    }
                    else
                    {
                        fallOff = 0;
                        float distance = Vector2.Distance(origin, pos);
                        if (distance > Radius)
                        {
                            fallOff = 255;
                            pastMaxDistance = true;
                        }
                        else
                        {
                            float normalized = distance / Radius;
                            normalized = Mathf.Pow(normalized, 2);
                            fallOff = (int)(normalized * 255);
                        }
                        Shadow projection = ProjectTile(row, col);

                        // Set the visibility of this cell
                        bool visible = !line.IsInShadow(projection);
                        if (level.SetVisibility(pos.x, pos.y, visible, fallOff))
                            ret.Add(pos);

                        // Add any opaque tiles to the shadow map
                        if (visible && level.Opaque(pos.x, pos.y))
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

        private static void DebugFOV(Level level, Vector2Int cell)
        {
            Debug.Visualisation.MarkPos(
                cell,
                level.Visible(cell.x, cell.y) ? Color.green : Color.red,
                1f);
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
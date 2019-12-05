// Helpers.cs
// Jerome Martina

using System;
using UnityEngine;
using Vector2Int = UnityEngine.Vector2Int;

namespace Pantheon.Utils
{
    /// <summary>
    /// Miscellaneous helper functions which belong nowhere else.
    /// </summary>
    public static class Helpers
    {
        public static Vector3 ToVector3(this Vector2Int v2i)
            => new Vector3(v2i.x, v2i.y);

        public static Vector2Int ToVector2Int(this Vector3Int v3i)
            => new Vector2Int(v3i.x, v3i.y);

        // Swap two references
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static CardinalDirection ToCardinal(this Vector2Int v2i)
        {
            if (v2i == new Vector2Int(0, 0))
                return CardinalDirection.Centre;
            else if (v2i.x == 0 && v2i.y >= 1)
                return CardinalDirection.North;
            else if (v2i.x >= 1 && v2i.y >= 1)
                return CardinalDirection.NorthEast;
            else if (v2i.x >= 1 && v2i.y == 0)
                return CardinalDirection.East;
            else if (v2i.x >= 1 && v2i.y <= -1)
                return CardinalDirection.SouthEast;
            else if (v2i.x == 0 && v2i.y <= -1)
                return CardinalDirection.South;
            else if (v2i.x <= -1 && v2i.y <= -1)
                return CardinalDirection.SouthWest;
            else if (v2i.x <= -1 && v2i.y == 0)
                return CardinalDirection.West;
            else if (v2i.x <= -1 && v2i.y >= 1)
                return CardinalDirection.NorthWest;
            else
                throw new ArgumentException("Argument vector invalid.");
        }

        public static Vector2Int CardinalToV2I(CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.Centre:
                    return new Vector2Int(0, 0);
                case CardinalDirection.North:
                    return new Vector2Int(0, 1);
                case CardinalDirection.NorthEast:
                    return new Vector2Int(1, 1);
                case CardinalDirection.East:
                    return new Vector2Int(1, 0);
                case CardinalDirection.SouthEast:
                    return new Vector2Int(1, -1);
                case CardinalDirection.South:
                    return new Vector2Int(0, -1);
                case CardinalDirection.SouthWest:
                    return new Vector2Int(-1, -1);
                case CardinalDirection.West:
                    return new Vector2Int(-1, 0);
                case CardinalDirection.NorthWest:
                    return new Vector2Int(-1, 1);
                default:
                    throw new ArgumentException("Bad CardinalDirection given.");
            }
        }

        public static CardinalDirection CardinalOpposite(CardinalDirection dir)
        {
            switch (dir)
            {
                case CardinalDirection.North:
                    return CardinalDirection.South;
                case CardinalDirection.NorthEast:
                    return CardinalDirection.SouthWest;
                case CardinalDirection.East:
                    return CardinalDirection.West;
                case CardinalDirection.SouthEast:
                    return CardinalDirection.NorthWest;
                case CardinalDirection.South:
                    return CardinalDirection.North;
                case CardinalDirection.SouthWest:
                    return CardinalDirection.NorthEast;
                case CardinalDirection.West:
                    return CardinalDirection.East;
                case CardinalDirection.NorthWest:
                    return CardinalDirection.SouthEast;
                case CardinalDirection.Centre:
                default:
                    throw new ArgumentException("Bad CardinalDirection given.");
            }
        }

        public static string Adjective(this CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.Centre:
                    return "Central";
                case CardinalDirection.North:
                    return "Northern";
                case CardinalDirection.NorthEast:
                    return "Northeastern";
                case CardinalDirection.East:
                    return "Eastern";
                case CardinalDirection.SouthEast:
                    return "Southeastern";
                case CardinalDirection.South:
                    return "Southern";
                case CardinalDirection.SouthWest:
                    return "Southwestern";
                case CardinalDirection.West:
                    return "Western";
                case CardinalDirection.NorthWest:
                    return "Northwestern";
                default:
                    throw new ArgumentException("Invalid case.");
            }
        }

        /// <summary>
        /// Extension method to check if an array has non-null elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns>True if arr has at least one non-null element.</returns>
        public static bool HasElements<T>(this T[] arr)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
                throw new ArgumentException($"Type {typeof(T)} is not nullable.");

            foreach (T t in arr)
                if (arr != null)
                    return true;

            return false;
        }

        /// <summary>
        /// Rotate a Vector2Int 90 degrees CW or CCW.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="ccw"></param>
        /// <returns></returns>
        public static void Rotate(ref this Vector2Int vector, bool ccw)
        {
            double rotX = vector.x;
            double rotY = vector.y;

            if (!ccw)
            {
                rotX = (
                    (vector.x * Math.Cos(1.5708)) -
                    (vector.y * Math.Sin(1.5708)));
                rotX = Math.Round(rotX);
                rotY = (
                    (vector.x * Math.Sin(1.5708)) + 
                    (vector.y * Math.Cos(1.5708)));
                rotY = Math.Round(rotY);
            }
            else
            {
                rotX = (int)(
                    (vector.x * Math.Cos(-1.5708)) -
                    (vector.y * Math.Sin(-1.5708)));
                rotX = Math.Round(rotX);
                rotY = (int)(
                    (vector.x * Math.Sin(-1.5708)) +
                    (vector.y * Math.Cos(-1.5708)));
                rotY = Math.Round(rotY);
            }

            vector.Set((int)rotX, (int)rotY);
        }

        public static Vector2Int Quotient(this Vector2Int vector, int divisor)
            => new Vector2Int(vector.x / divisor, vector.y / divisor);
    }
}


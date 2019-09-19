// Helpers.cs
// Jerome Martina

using System;
using UnityEngine;

namespace Pantheon.Utils
{
    /// <summary>
    /// Miscellaneous helper functions which belong nowhere else.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Vector2Int to Vector3.
        /// </summary>
        /// <param name="v2i"></param>
        /// <returns></returns>
        public static Vector3 V2IToV3(Vector2Int v2i)
            => new Vector3(v2i.x, v2i.y);

        /// <summary>
        /// Vector3Int to Vector2Int.
        /// </summary>
        /// <param name="v3i"></param>
        /// <returns></returns>
        public static Vector2Int V3IToV2I(Vector3Int v3i)
            => new Vector2Int(v3i.x, v3i.y);

        // Swap two references
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static CardinalDirection V2IToCardinal(Vector2Int v2i)
        {
            if (v2i == new Vector2Int(0, 0))
                return CardinalDirection.Centre;
            else if (v2i == new Vector2Int(0, 1))
                return CardinalDirection.North;
            else if (v2i == new Vector2Int(1, 1))
                return CardinalDirection.NorthEast;
            else if (v2i == new Vector2Int(1, 0))
                return CardinalDirection.East;
            else if (v2i == new Vector2Int(1, -1))
                return CardinalDirection.SouthEast;
            else if (v2i == new Vector2Int(0, -1))
                return CardinalDirection.South;
            else if (v2i == new Vector2Int(-1, -1))
                return CardinalDirection.SouthWest;
            else if (v2i == new Vector2Int(-1, 0))
                return CardinalDirection.West;
            else if (v2i == new Vector2Int(-1, 1))
                return CardinalDirection.NorthWest;
            else
                throw new ArgumentException("Argument vector not normalized.");
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
    }
}


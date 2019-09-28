// Connect.cs
// Jerome Martina

using System;
using UnityEngine;
using Pantheon.World;

namespace Pantheon.WorldGen
{
    public static class Connect
    {
        public static void ConnectByDirection(Level level,
            CardinalDirection direction)
        {
            if (direction.HasFlag(CardinalDirection.Centre))
                throw new ArgumentException
                    ("Trails cannot be run towards centre.");

            if (direction.HasFlag(CardinalDirection.North))
            {
                Cell trailNorthCell = level.RandomFloorInDirection
                    (CardinalDirection.North);
                Connection trailNorth = new Connection(level.Layer, level,
                    trailNorthCell, FeatureType.TrailNorth,
                    new Vector2Int(0, 1));
                level.LateralConnections.Add(CardinalDirection.North, trailNorth);
            }

            if (direction.HasFlag(CardinalDirection.East))
            {
                Cell trailEastCell = level.RandomFloorInDirection
                    (CardinalDirection.East);
                Connection trailEast = new Connection(level.Layer, level,
                    trailEastCell, FeatureType.TrailEast,
                    new Vector2Int(1, 0));
                level.LateralConnections.Add(CardinalDirection.East,
                    trailEast);
            }

            if (direction.HasFlag(CardinalDirection.South))
            {
                Cell trailSouthCell = level.RandomFloorInDirection
                    (CardinalDirection.South);
                Connection trailSouth = new Connection(level.Layer, level,
                    trailSouthCell, FeatureType.TrailSouth,
                    new Vector2Int(0, -1));
                level.LateralConnections.Add(CardinalDirection.South, trailSouth);
            }

            if (direction.HasFlag(CardinalDirection.West))
            {
                Cell trailWestCell = level.RandomFloorInDirection
                    (CardinalDirection.West);
                Connection trailWest = new Connection(level.Layer, level,
                    trailWestCell, FeatureType.TrailWest,
                    new Vector2Int(-1, 0));
                level.LateralConnections.Add(CardinalDirection.West, trailWest);
            }
        }

        /// <summary>
        /// Connect levels so as to form a ring around a zone.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="wing"></param>
        /// <param name="theme">To decide FeatureType.</param>
        public static void ConnectZone(Level level, CardinalDirection wing, 
           ZoneTheme theme)
        {
            // XXX: This should be dynamic
            switch (wing)
            {
                case CardinalDirection.Centre:
                    {
                        // TODO: Path to boss level needs to depend on layouts
                        // of other levels

                        Cell trailWestCell = level.RandomFloorInDirection
                            (CardinalDirection.West);
                        Connection trailWest = new Connection(level.Layer,
                            level, trailWestCell, FeatureType.TrailWest,
                            new Vector2Int(-1, 0));
                        level.LateralConnections.Add(CardinalDirection.West,
                            trailWest);

                        break;
                    }
                case CardinalDirection.North:
                case CardinalDirection.South:
                    {
                        Cell trailWestCell = level.RandomFloorInDirection
                            (CardinalDirection.West);
                        Connection trailWest = new Connection(level.Layer,
                            level, trailWestCell, FeatureType.TrailWest,
                            new Vector2Int(-1, 0));
                        level.LateralConnections.Add(CardinalDirection.West,
                            trailWest);

                        Cell trailEastCell = level.RandomFloorInDirection
                            (CardinalDirection.East);
                        Connection trailEast = new Connection(level.Layer,
                            level, trailEastCell, FeatureType.TrailEast,
                            new Vector2Int(1, 0));
                        level.LateralConnections.Add(CardinalDirection.East,
                            trailEast);

                        break;
                    }
                case CardinalDirection.West:
                    {
                        // Connect to centre
                        Cell trailEastCell = level.RandomFloorInDirection
                            (CardinalDirection.East);
                        Connection trailEast = new Connection(level.Layer,
                            level, trailEastCell, FeatureType.TrailEast,
                            new Vector2Int(1, 0));
                        level.LateralConnections.Add(CardinalDirection.East,
                            trailEast);

                        goto case CardinalDirection.East;
                    }
                case CardinalDirection.East:
                    {
                        Cell trailNorthCell = level.RandomFloorInDirection
                            (CardinalDirection.North);
                        Connection trailNorth = new Connection(level.Layer,
                            level, trailNorthCell, FeatureType.TrailNorth,
                            new Vector2Int(0, 1));
                        level.LateralConnections.Add(CardinalDirection.North,
                            trailNorth);

                        Cell trailSouthCell = level.RandomFloorInDirection
                            (CardinalDirection.South);
                        Connection trailSouth = new Connection(level.Layer,
                            level, trailSouthCell, FeatureType.TrailSouth,
                            new Vector2Int(0, -1));
                        level.LateralConnections.Add(CardinalDirection.South,
                            trailSouth);

                        break;
                    }
                case CardinalDirection.NorthEast:
                    {
                        Cell trailWestCell = level.RandomFloorInDirection
                            (CardinalDirection.West);
                        Connection trailWest = new Connection(level.Layer,
                            level, trailWestCell, FeatureType.TrailWest,
                            new Vector2Int(-1, 0));
                        level.LateralConnections.Add(CardinalDirection.West,
                            trailWest);

                        Cell trailSouthCell = level.RandomFloorInDirection
                            (CardinalDirection.South);
                        Connection trailSouth = new Connection(level.Layer,
                            level, trailSouthCell, FeatureType.TrailSouth,
                            new Vector2Int(0, -1));
                        level.LateralConnections.Add(CardinalDirection.South,
                            trailSouth);

                        break;
                    }
                case CardinalDirection.SouthEast:
                    {
                        Cell trailNorthCell = level.RandomFloorInDirection
                            (CardinalDirection.North);
                        Connection trailNorth = new Connection(level.Layer,
                            level, trailNorthCell, FeatureType.TrailNorth,
                            new Vector2Int(0, 1));
                        level.LateralConnections.Add(CardinalDirection.North,
                            trailNorth);

                        Cell trailWestCell = level.RandomFloorInDirection
                            (CardinalDirection.West);
                        Connection trailWest = new Connection(level.Layer,
                            level, trailWestCell, FeatureType.TrailWest,
                            new Vector2Int(-1, 0));
                        level.LateralConnections.Add(CardinalDirection.West,
                            trailWest);

                        break;
                    }
                case CardinalDirection.SouthWest:
                    {
                        Cell trailNorthCell = level.RandomFloorInDirection
                            (CardinalDirection.North);
                        Connection trailNorth = new Connection(level.Layer,
                            level, trailNorthCell, FeatureType.TrailNorth,
                            new Vector2Int(0, 1));
                        level.LateralConnections.Add(CardinalDirection.North,
                            trailNorth);

                        Cell trailEastCell = level.RandomFloorInDirection
                            (CardinalDirection.East);
                        Connection trailEast = new Connection(level.Layer,
                            level, trailEastCell, FeatureType.TrailEast,
                            new Vector2Int(1, 0));
                        level.LateralConnections.Add(CardinalDirection.East,
                            trailEast);

                        break;
                    }
                case CardinalDirection.NorthWest:
                    {
                        Cell trailSouthCell = level.RandomFloorInDirection
                            (CardinalDirection.South);
                        Connection trailSouth = new Connection(level.Layer,
                            level, trailSouthCell, FeatureType.TrailSouth,
                            new Vector2Int(0, -1));
                        level.LateralConnections.Add(CardinalDirection.South,
                            trailSouth);

                        Cell trailEastCell = level.RandomFloorInDirection
                            (CardinalDirection.East);
                        Connection trailEast = new Connection(level.Layer,
                            level, trailEastCell, FeatureType.TrailEast,
                            new Vector2Int(1, 0));
                        level.LateralConnections.Add(CardinalDirection.East,
                            trailEast);

                        break;
                    }
                default:
                    throw new ArgumentException("Illegal wing.");
            }
        }

        /// <summary>
        /// Place connections on either end of a transition level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void TransitionConnect(Level level,
            CardinalDirection start, CardinalDirection end)
        {
            if ((start == CardinalDirection.North && 
                end == CardinalDirection.South) || 
                (start == CardinalDirection.South && 
                end == CardinalDirection.North))
            {
                Cell trailNorthCell = level.RandomFloorOnY
                            (level.LevelSize.y - 2, true);
                Connection trailNorth = new Connection(level.Layer,
                    level, trailNorthCell, FeatureType.TrailNorth,
                    new Vector2Int(0, 1));
                level.LateralConnections.Add(CardinalDirection.North,
                    trailNorth);

                Cell trailSouthCell = level.RandomFloorOnY(1, true);
                Connection trailSouth = new Connection(level.Layer,
                    level, trailSouthCell, FeatureType.TrailSouth,
                    new Vector2Int(0, -1));
                level.LateralConnections.Add(CardinalDirection.South,
                    trailSouth);
            }
            else if ((start == CardinalDirection.East && 
                end == CardinalDirection.West) || 
                (start == CardinalDirection.West && 
                end == CardinalDirection.East))
            {
                Cell trailWestCell = level.RandomFloorOnX(1, true);
                Connection trailWest = new Connection(level.Layer,
                    level, trailWestCell, FeatureType.TrailWest,
                    new Vector2Int(-1, 0));
                level.LateralConnections.Add(CardinalDirection.West,
                    trailWest);

                Cell trailEastCell = level.RandomFloorOnX
                    (level.LevelSize.x - 2, true);
                Connection trailEast = new Connection(level.Layer,
                    level, trailEastCell, FeatureType.TrailEast,
                    new Vector2Int(1, 0));
                level.LateralConnections.Add(CardinalDirection.East,
                    trailEast);
            }
        }
    }
}

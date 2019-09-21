// Connect.cs
// Jerome Martina

using UnityEngine;
using Pantheon.World;

namespace Pantheon.WorldGen
{
    public static class Connect
    {
        public static void Trails(Level level, CardinalDirection directions)
        {
            if (directions.HasFlag(CardinalDirection.Centre))
                throw new System.ArgumentException
                    ("Trails cannot be run towards centre.");

            if (directions.HasFlag(CardinalDirection.North))
            {
                Cell trailNorthCell = level.RandomFloorOnY
                    (level.LevelSize.y - 2, true);
                Connection trailNorth = new Connection(level.Layer, level,
                    trailNorthCell, FeatureType.TrailNorth,
                    new Vector2Int(0, 1));
                trailNorthCell.Connection = trailNorth;
                level.LateralConnections.Add(CardinalDirection.North, trailNorth);
            }

            if (directions.HasFlag(CardinalDirection.East))
            {
                Cell trailEastCell = level.RandomFloorOnX
                    (level.LevelSize.x - 2, true);
                Connection trailEast = new Connection(level.Layer, level,
                    trailEastCell, FeatureType.TrailEast,
                    new Vector2Int(1, 0));
                trailEastCell.Connection = trailEast;
                level.LateralConnections.Add(CardinalDirection.East, trailEast);
            }

            if (directions.HasFlag(CardinalDirection.South))
            {
                Cell trailSouthCell = level.RandomFloorOnY(1, true);
                Connection trailSouth = new Connection(level.Layer, level,
                    trailSouthCell, FeatureType.TrailSouth,
                    new Vector2Int(0, -1));
                trailSouthCell.Connection = trailSouth;
                level.LateralConnections.Add(CardinalDirection.South, trailSouth);
            }

            if (directions.HasFlag(CardinalDirection.West))
            {
                Cell trailWestCell = level.RandomFloorOnX(1, true);
                Connection trailWest = new Connection(level.Layer, level,
                    trailWestCell, FeatureType.TrailWest,
                    new Vector2Int(-1, 0));
                trailWestCell.Connection = trailWest;
                level.LateralConnections.Add(CardinalDirection.West, trailWest);
            }
        }
    }
}

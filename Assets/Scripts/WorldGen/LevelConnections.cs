// LevelConnections.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.World;

namespace Pantheon.WorldGen
{
    public static class LevelConnections
    {
        public static Connection MapEdgeConnection(
            Level level,
            CardinalDirection direction,
            FeatureType featureType,
            LateralConnection.FirstTravelDelegate onFirstTravel)
        {
            Cell cell;

            switch (direction)
            {
                case CardinalDirection.North:
                    cell = level.RandomFloor(-1, level.LevelSize.y - 2);
                    break;
                case CardinalDirection.NorthEast:
                    cell = level.Map[level.LevelSize.x - 2, level.LevelSize.y - 2];
                    break;
                case CardinalDirection.East:
                    cell = level.RandomFloor(level.LevelSize.x - 2, -1);
                    break;
                case CardinalDirection.SouthEast:
                    cell = level.Map[level.LevelSize.x - 2, 1];
                    break;
                case CardinalDirection.South:
                    cell = level.RandomFloor(-1, 1);
                    break;
                case CardinalDirection.SouthWest:
                    cell = level.Map[1, 1];
                    break;
                case CardinalDirection.West:
                    cell = level.RandomFloor(1, -1);
                    break;
                case CardinalDirection.NorthWest:
                    cell = level.Map[1, level.LevelSize.y - 2];
                    break;
                default:
                    throw new System.Exception("Bad direction given.");
            }

            Connection conn = new LateralConnection(
            level, cell, Database.GetFeature(featureType),
            onFirstTravel, direction);

            cell.Connection = conn;

            return conn;
        }

        public static Connection ConnectZones(
            Level newLevel,
            Cell newCell,
            string levelRef,
            string connRef,
            FeatureType featureType)
        {
            if (!Game.instance.levels.TryGetValue(levelRef, out Level oldLevel))
                throw new System.Exception
                    ($"Level \"{levelRef}\" was not generated, or has bad ref.");

            if (!oldLevel.Connections.TryGetValue(connRef, out Connection oldConn))
                throw new System.Exception($"{oldLevel} has no connection \"{connRef}\".");

            Connection newConn = new LateralConnection(
                newLevel, 
                newCell, 
                Database.GetFeature(featureType), 
                oldConn);
            newCell.Connection = newConn;

            return newConn;
        }
    }
}

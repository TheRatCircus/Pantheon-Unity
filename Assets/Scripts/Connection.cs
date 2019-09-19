// Connection.cs
// Jerome Martina

#define DEBUG_CONNECTIONS
#undef DEBUG_CONNECTIONS

using System;
using UnityEngine;
using Pantheon.Core;
using Pantheon.Actors;

namespace Pantheon.World
{
    public class Connection
    {
        public Action<Connection> ConnectDelegate;
        public Action<Connection, Vector2Int> LateralConnectDelegate;

        public string DisplayName { get; set; }

        public Layer Layer { get; }
        public Level Level { get; }
        public Cell Cell { get; }

        public Layer DestinationLayer { get; set; }
        public Level DestinationLevel { get; set; }
        public Cell DestinationCell { get; set; }

        public LevelRef DestinationRef { get; set; }
        public Vector2Int DestinationVector { get; set; }

        public Connection(Level level, Cell cell,
            FeatureType feature, LevelRef destinationRef)
        {
            Level = level;
            Cell = cell;
            Cell.SetFeature(Database.GetFeature(feature));
            DestinationRef = destinationRef;
        }

        public Connection(Layer layer, Level level, Cell cell,
            FeatureType feature, Vector2Int destinationVector)
        {
            Layer = layer;
            Level = level;
            Cell = cell;
            Cell.SetFeature(Database.GetFeature(feature));
            DestinationVector = destinationVector;
        }

        public void SetDestination(Connection other)
        {
            DestinationLayer = other.Layer;
            DestinationLevel = other.Level;
            DestinationCell = other.Cell;

            other.DestinationLayer = Layer;
            other.DestinationLevel = Level;
            other.DestinationCell = Cell;
        }

        public void Travel(Player player)
        {
            if (DestinationLevel == null)
            {
                if (DestinationVector != null)
                    DestinationLevel = Layer.RequestLevel(Level.LayerPos + DestinationVector);
                else
                    DestinationLevel = Game.instance.RequestLevel(DestinationRef);
            }
            Game.instance.MoveToLevel(player, DestinationLevel, DestinationCell);
        }
    }
}

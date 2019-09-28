// Connection.cs
// Jerome Martina

#define DEBUG_CONNECTIONS
#undef DEBUG_CONNECTIONS

using Pantheon.Actors;
using Pantheon.Core;
using System;
using UnityEngine;

namespace Pantheon.World
{
    public sealed class Connection
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

        public string DestinationRef { get; set; }
        public Vector2Int DestinationVector { get; set; }
        public bool OneWay { get; set; }

        public Connection(Level level, Cell cell,
            FeatureType feature, string destinationRef)
        {
            Level = level;
            Cell = cell;
            Cell.SetFeature(feature);
            DestinationRef = destinationRef;
            cell.Connection = this;
        }

        public Connection(Layer layer, Level level, Cell cell,
            FeatureType feature, Vector2Int destinationVector)
        {
            Layer = layer;
            Level = level;
            Cell = cell;
            Cell.SetFeature(feature);
            DestinationVector = destinationVector;
            cell.Connection = this;
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
                if (DestinationRef != null)
                {
                    if (OneWay)
                    {
                        DestinationLevel = Game.instance.RequestLevel(null,
                            DestinationRef);
                        DestinationCell = DestinationLevel.RandomFloor();
                    }
                    else
                        DestinationLevel = Game.instance.RequestLevel
                            (Level.RefName, DestinationRef);
                }
                else
                    DestinationLevel = Layer.RequestLevel
                        (Level.LayerPos + DestinationVector);
            }
            Game.instance.MoveToLevel(player, DestinationLevel, 
                DestinationCell);
        }
    }
}

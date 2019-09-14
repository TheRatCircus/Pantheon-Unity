// Connection.cs
// Jerome Martina

#define DEBUG_CONNECTIONS
#undef DEBUG_CONNECTIONS

using Pantheon.Core;
using Pantheon.Actors;

namespace Pantheon.World
{
    /// <summary>
    /// Represents a point by which one level can be travelled to from another.
    /// </summary>
    public abstract class Connection
    {
        public Level Level { get; }
        public Cell Cell { get; }
        public Connection Partner { get; private set; }

        public Connection(Level level, Cell cell, Feature feature)
        {
            Level = level;
            Cell = cell;
            Cell.SetFeature(feature);
        }

        /// <summary>
        /// Generate a connection with a pre-existing partner in another level.
        /// </summary>
        /// <param name="level">Level housing this connection.</param>
        /// <param name="cell">Cell housing this connection.</param>
        /// <param name="feature">Feature representing this connection.</param>
        /// <param name="partner">This connection's partner.</param>
        public Connection(Level level, Cell cell, Feature feature,
            Connection partner)
        {
            Level = level;
            Cell = cell;
            Cell.SetFeature(feature);
            // Partnership is mutual
            Partner = partner;
            partner.Partner = this;
        }

        public abstract void Use(Player player);

        [System.Diagnostics.Conditional("DEBUG_CONNECTIONS")]
        protected void LogTravel(Actor actor, Level level, Cell cell)
        {
            UnityEngine.Debug.Log
                ($"{actor.ActorName} is travelling to " +
                $"{cell.ToString()} in " +
                $"{level.RefName}.");
        }
    }

    /// <summary>
    /// A connection between multiple wings of a zone.
    /// </summary>
    public class LateralConnection : Connection
    {
        public delegate void FirstTravelDelegate(ref Level level, CardinalDirection wing);

        public FirstTravelDelegate onFirstTravel;
        private CardinalDirection destinationWing;

        public LateralConnection(
            Level level, Cell cell, Feature feature,
            FirstTravelDelegate onFirstTravel, CardinalDirection destinationWing)
            : base(level, cell, feature)
        {
            this.onFirstTravel = onFirstTravel;
            this.destinationWing = destinationWing;
        }

        public LateralConnection(
            Level level, Cell cell, Feature feature, Connection partner)
            : base(level, cell, feature, partner) { }

        // Travel by way of this connection
        public override void Use(Player player)
        {
            if (Partner == null)
            {
                Level destinationLevel = Game.instance.MakeNewLevel();
                onFirstTravel?.Invoke(ref destinationLevel, destinationWing);
            }
            
            LogTravel(player, Partner.Level, Partner.Cell);
            Game.instance.MoveToLevel(player, Partner.Level, Partner.Cell);
        }
    }

    /// <summary>
    /// A connection between multiple floors of a zone.
    /// </summary>
    public class VerticalConnection : Connection
    {
        public delegate void FirstTravelDelegate(ref Level level, int depth);

        public FirstTravelDelegate onFirstTravel;
        private int destinationDepth;

        public VerticalConnection(
            Level level, Cell cell, Feature feature,
            FirstTravelDelegate onFirstUse, int destinationDepth)
            : base(level, cell, feature)
        {
            this.onFirstTravel = onFirstUse;
            this.destinationDepth = destinationDepth;
        }

        // Travel by way of this connection
        public override void Use(Player player)
        {
            if (Partner == null)
            {
                Level destinationLevel = Game.instance.MakeNewLevel();
                onFirstTravel?.Invoke(ref destinationLevel, destinationDepth);
            }

            LogTravel(player, Partner.Level, Partner.Cell);
            Game.instance.MoveToLevel(player, Partner.Level, Partner.Cell);
        }
    }
}

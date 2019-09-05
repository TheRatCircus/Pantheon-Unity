// Connection.cs
// Jerome Martina

/// <summary>
/// Represents a point by which one level can be travelled to from another.
/// </summary>
public abstract class Connection
{
    protected Level destinationLevel;

    public Level OriginLevel { get; }
    public Level DestinationLevel
    { get => destinationLevel; protected set => destinationLevel = value; }

    public Cell OriginCell { get; }
    public Cell DestinationCell { get; protected set; }

    public Connection(Level originLevel, Cell originCell, Feature feature)
    {
        OriginLevel = originLevel;
        OriginCell = originCell;
        OriginCell.Feature = feature;
    }

    /// <summary>
    /// Used to generate partner connections on first travel.
    /// </summary>
    /// <param name="connection"></param>
    protected Connection(Connection connection, Feature feature)
    {
        DestinationLevel = connection.OriginLevel;
        DestinationCell = connection.OriginCell;

        OriginLevel = connection.destinationLevel;
        OriginCell = connection.DestinationCell;

        OriginCell.Feature = feature;
    }

    public abstract void Use(Player player);
}

/// <summary>
/// A connection between multiple wings of a zone.
/// </summary>
public class LateralConnection : Connection
{
    public delegate void FirstUseDelegate(ref Level level, CardinalDirection wing);

    public FirstUseDelegate onFirstUse;
    private CardinalDirection destinationWing;

    public LateralConnection(
        Level originLevel, Cell originCell, Feature feature,
        FirstUseDelegate onFirstUse, CardinalDirection destinationWing)
        : base(originLevel, originCell, feature)
    {
        this.onFirstUse = onFirstUse;
        this.destinationWing = destinationWing;
    }

    protected LateralConnection(Connection connection, Feature feature)
        : base(connection, feature) { }

    // Travel by way of this connection
    public override void Use(Player player)
    {
        if (DestinationLevel == null)
        {
            DestinationLevel = Game.instance.MakeNewLevel();
            onFirstUse?.Invoke(ref destinationLevel, destinationWing);
            DestinationCell = DestinationLevel.RandomFloor();
            DestinationCell.Connection = new LateralConnection
                (this, Database.GetFeature(FeatureType.StairsUp));
        }

        Game.instance.MoveToLevel(player, destinationLevel, DestinationCell);
    }
}

/// <summary>
/// A connection between multiple floors of a zone.
/// </summary>
public class VerticalConnection : Connection
{
    public delegate void FirstUseDelegate(ref Level level, int depth);

    public FirstUseDelegate onFirstUse;
    private int destinationDepth;

    public VerticalConnection(
        Level originLevel, Cell originCell, Feature feature,
        FirstUseDelegate onFirstUse, int destinationDepth)
        : base(originLevel, originCell, feature)
    {
        this.onFirstUse = onFirstUse;
        this.destinationDepth = destinationDepth;
    }

    protected VerticalConnection(Connection connection, Feature feature)
        : base(connection, feature) { }

    // Travel by way of this connection
    public override void Use(Player player)
    {
        if (DestinationLevel == null)
        {
            DestinationLevel = Game.instance.MakeNewLevel();
            onFirstUse?.Invoke(ref destinationLevel, destinationDepth);
            Cell cell = DestinationLevel.RandomFloor();
            DestinationCell = cell;
        }

        Game.instance.MoveToLevel(player, destinationLevel, DestinationCell);
    }
}
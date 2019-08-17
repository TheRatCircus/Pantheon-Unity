// Cell data
using System.Collections.Generic;
using UnityEngine;

// Types of terrain features which can exist in a cell
public enum FeatureType
{
    None = 0,
    StairsUp = 1,
    StairsDown = 2
}

public class Cell
{
    // Statics
    // The offset of each tile from Unity's true grid coords
    public const float TileOffsetX = .5f;
    public const float TileOffsetY = .5f;

    private Vector2Int position;

    TerrainData terrainData;
    bool blocked = true; // Can cell be moved through?
    bool opaque = true; // Can cell be seen through?
    FeatureType feature = FeatureType.None;
    Connection connection;

    // Status
    bool visible = false; // Is this cell within view?
    bool revealed = false; // Is this cell known?

    // Contents of cell
    Actor actor = null;
    List<Item> items = new List<Item>();

    // Properties
    public Vector2Int Position { get => position; set => position = value; }
    public bool Blocked { get => blocked; set => blocked = value; }
    public bool Opaque { get => opaque; set => opaque = value; }
    public bool Visible
    {
        get => visible;
    }
    public bool Revealed { get => revealed; set => revealed = value; }
    public Actor _actor { get => actor; set => actor = value; }
    public List<Item> Items { get => items; }
    public TerrainData _terrainData { get => terrainData; }
    public FeatureType Feature { get => feature; set => feature = value; }
    public Connection _connection { get => connection; set => connection = value; }

    // Constructor
    public Cell(Vector2Int position)
    {
        this.position = position;
    }

    // Set this cell's visibility
    public void SetVisibility(bool visible, int fallOff)
    {
        if (!visible)
        {
            this.visible = false;
            return;
        }
        else
        {
            if (fallOff > 100)
                this.visible = false;
            else
            {
                this.visible = true;
                revealed = true;
            }
        }
    }
    
    // Add or remove a connection point
    public void SetConnection(bool upstairs)
    {
        connection = new Connection(upstairs);
        if (upstairs)
            feature = FeatureType.StairsUp;
        else
            feature = FeatureType.StairsDown;
    }

    // Set this cell's terrain type and adjust its attributes accordingly
    public void SetTerrainType(TerrainData terrainData)
    {
        this.terrainData = terrainData;
        opaque = terrainData.Opaque;
        blocked = terrainData.Blocked;
    }

    // Check if this cell can be walked into
    public bool IsWalkable()
    {
        return (!blocked && actor == null);
    }

    // toString override: returns name of tile, position, contained actor
    public override string ToString()
    {
        string ret = $"{(visible ? "Visible" : "Unseen")} {(revealed ? terrainData.DisplayName : "Unknown terrain")} at {position}";
        //string ret = $"{(revealed ? terrainData.DisplayName : "Unknown terrain")} at {position}";
        return ret;
    }
}

public class Connection
{
    public bool upstairs;
    public Level DestinationLevel;
    public Cell DestinationCell;

    public Connection(bool upstairs) { this.upstairs = upstairs; }

    // Travel by way of this connection
    public void GoToLevel(Player player)
    {
        if (DestinationLevel == null)
        {
            DestinationLevel = Game.instance.MakeNewLevel();
            DestinationLevel.Initialize(false);
            Cell cell = DestinationLevel.RandomFloor();
            DestinationCell = cell;
        }
        player.transform.SetParent(DestinationLevel.transform);
        player.level = DestinationLevel;
        player.MoveToCell(DestinationCell);
        Game.instance.LoadLevel(DestinationLevel);
        DestinationLevel.RefreshFOV();
    }
}

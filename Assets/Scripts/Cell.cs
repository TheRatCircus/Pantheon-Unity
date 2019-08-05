// Cell data
using System.Collections.Generic;
using UnityEngine;

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
        set
        {
            visible = value;
            // If made visible, reveal but never unreveal
            if (value) Revealed = true;
        }
    }
    public bool Revealed { get => revealed; set => revealed = value; }
    public Actor _actor { get => actor; set => actor = value; }
    public List<Item> Items { get => items; }
    public TerrainData _terrainData { get => terrainData; }

    // Constructor
    public Cell(Vector2Int position)
    {
        this.position = position;
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
        string ret = $"{(revealed ? terrainData.DisplayName : "Unknown terrain")} at {position}";
        return ret;
    }
}

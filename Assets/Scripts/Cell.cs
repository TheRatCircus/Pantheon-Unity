// Cell data
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    // Statics
    // The offset of each tile from Unity's true grid coords
    public const float TileOffsetX = .5f;
    public const float TileOffsetY = .5f;

    [SerializeField] private Vector2Int position;

    TerrainData terrainData;
    bool blocked = true; // Can cell be moved through?
    bool opaque = true; // Can cell be seen through?
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
    public bool Opaque { get => opaque && Feature.Opaque; set => opaque = value; }
    public bool Visible => visible;
    public bool Revealed { get => revealed; set => revealed = value; }
    public Actor _actor { get => actor; set => actor = value; }
    public List<Item> Items => items;
    public TerrainData TerrainData { get => terrainData; }
    public Feature Feature { get; set; } = null;
    public Connection Connection { get => connection; set => connection = value; }

    // Constructor
    public Cell(Vector2Int position) => this.position = position;

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

    // Set this cell's terrain type and adjust its attributes accordingly
    public void SetTerrainType(TerrainData terrainData)
    {
        this.terrainData = terrainData;
        opaque = terrainData.Opaque;
        blocked = terrainData.Blocked;
    }

    // Check if this cell can be walked into
    public bool IsWalkableTerrain() => !blocked && !Feature.Blocked;

    // toString override: returns name of tile, position, contained actor
    public override string ToString()
    {
        string ret = $"{(visible ? "Visible" : "Unseen")} {(revealed ? terrainData.DisplayName : "Unknown terrain")} at {position}";
        //string ret = $"{(revealed ? terrainData.DisplayName : "Unknown terrain")} at {position}";
        return ret;
    }
}
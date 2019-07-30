// Cell data
using UnityEngine;

public class Cell
{
    private Vector2Int position;

    bool blocked = true; // Can cell be moved through?
    bool opaque = true; // Can cell be seen through?

    // Status
    bool visible = false; // Is this cell within view?
    bool revealed = false; // Is this cell known?

    // Contents of cell
    Actor actor = null;

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
    public Actor _Actor { get => actor; set => actor = value; }

    // Constructor
    public Cell(Vector2Int position)
    {
        this.position = position;
    }

    // Check if this cell can be walked into
    public bool IsWalkable()
    {
        return (!blocked && actor == null);
    }

    // toString override: returns name of tile, position, contained actor
    public override string ToString()
    {
        string ret = $"{(visible ? "Visible" : "Unseen")} {(opaque ? "Wall" : "Floor")} at {position}" +
            $" containing {(actor != null ? actor.ToString() : "nothing")}";
        return ret;
    }
}

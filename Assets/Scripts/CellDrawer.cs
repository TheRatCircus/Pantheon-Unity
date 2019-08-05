// Draws tilemaps based on cell data
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public static class CellDrawer
{
    // Draw a single cell
    public static void DrawCell(Level level, Cell cell)
    {
        DrawTile(level, cell);
    }

    // Draw a list of cells
    public static void DrawCells(Level level, List<Cell> cells)
    {
        foreach (Cell cell in cells)
            DrawTile(level, cell);
    }

    // Draw the whole level
    public static void DrawLevel(Level level)
    {
        foreach (Cell cell in level.Cells)
            DrawTile(level, cell);
    }

    // Draw a cell's tile
    public static void DrawTile(Level level, Cell cell)
    {
        if (!cell.Revealed)
            level.terrainTilemap.SetTile((Vector3Int)cell.Position, level.unknownTile);
        else
        {
            level.terrainTilemap.SetTile((Vector3Int)cell.Position, cell._terrainData._tile);
            level.terrainTilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
            if (cell.Visible && cell.Items.Count > 0)
                if (cell._actor == null)
                {
                    Tile itemTile = ScriptableObject.CreateInstance<Tile>();
                    itemTile.sprite = cell.Items[0]._sprite;
                    if (itemTile.sprite != null)
                        level.itemTilemap.SetTile((Vector3Int)cell.Position, itemTile);
                    else
                        throw new System.Exception("Attempted to draw an item with no sprite");
                }
                else
                    level.itemTilemap.SetTile((Vector3Int)cell.Position, null);
        }
    }
}

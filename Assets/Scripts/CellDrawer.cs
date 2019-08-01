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
            level.Tilemap.SetTile((Vector3Int)cell.Position, level.unknownTile);
        else
        {
            level.Tilemap.SetTile((Vector3Int)cell.Position,
                cell.Blocked ? level.wallTile : level.groundTile);
            level.Tilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
        }
    }
}

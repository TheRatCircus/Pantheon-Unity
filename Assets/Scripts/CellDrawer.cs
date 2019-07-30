// Draws tilemaps based on cell data
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CellDrawer : MonoBehaviour
{
    public Tilemap tilemap;
    public Level level;

    // TODO: Cleanup

    // Draw a single cell
    public void DrawCell(Cell cell)
    {
        DrawTile(cell);
    }

    // Draw a list of cells
    public void DrawCells(List<Cell> cells)
    {
        foreach (Cell cell in cells)
        {
            DrawTile(cell);
        }
    }

    // Draw the whole level
    public void DrawLevel(Level level)
    {
        foreach (Cell cell in level.Cells)
        {
            DrawTile(cell);
        }
    }

    // Draw a cell's tile
    public void DrawTile(Cell cell)
    {
        if (!cell.Revealed)
            tilemap.SetTile((Vector3Int)cell.Position, level.unknownTile);
        else
        {
            tilemap.SetTile((Vector3Int)cell.Position,
                cell.Blocked ? level.wallTile : level.groundTile);
            tilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
        }
    }
}

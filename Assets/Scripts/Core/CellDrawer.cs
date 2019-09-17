// Draws tilemaps based on cell data

#define DEBUG_CELLDRAW
#undef DEBUG_CELLDRAW

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.Core;
using Pantheon.World;
using Pantheon.Utils;

public static class CellDrawer
{
    public static void DrawCell(Level level, Cell cell)
    {
        DrawTile(level, cell);
    }

    public static void DrawCells(Level level, List<Cell> cells)
    {
        LogCellDrawing(cells.Count);
        foreach (Cell cell in cells)
        {
            DrawTile(level, cell);
            Vector3 start = Helpers.V2IToV3(cell.Position);
            Vector3 end = new Vector3(start.x + .2f, start.y + .2f);
            Debug.DrawLine(start, end, Color.cyan, 5);
        }
    }

    [System.Diagnostics.Conditional("DEBUG_CELLDRAW")]
    public static void LogCellDrawing(int cellsDrawn)
    {
        Debug.Log($"Drawing {cellsDrawn} cells...");
    }

    public static void DrawLevel(Level level)
    {
        foreach (Cell cell in level.Map)
            DrawTile(level, cell);
    }

    public static void DrawTile(Level level, Cell cell)
    {
        if (!cell.Revealed)
            level.TerrainTilemap.SetTile((Vector3Int)cell.Position,
                Database.UnknownTerrain);
        else
        {
            DrawTerrain(level, cell);

            if (cell.Feature != null)
                DrawFeature(level, cell);

            if (cell.Revealed && cell.Items.Count > 0)
                if (cell.Actor == null)
                    DrawItem(level, cell);
                else
                    level.ItemTilemap.SetTile((Vector3Int)cell.Position, null);
        }
    }

    public static void DrawTerrain(Level level, Cell cell)
    {
        level.TerrainTilemap.SetTile((Vector3Int)cell.Position, cell.TerrainData._tile);
        level.TerrainTilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
    }

    public static void DrawFeature(Level level, Cell cell)
    {
        Tile featureTile = ScriptableObject.CreateInstance<Tile>();
        featureTile.flags = TileFlags.None;

        if (cell.Feature.Sprite != null)
            featureTile.sprite = cell.Feature.Sprite;
        else
            throw new NullReferenceException($"Feature {cell.Feature.name} has no sprite.");

        level.FeatureTilemap.SetTile((Vector3Int)cell.Position, featureTile);
        level.FeatureTilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
    }

    public static void DrawItem(Level level, Cell cell)
    {
        Tile itemTile = ScriptableObject.CreateInstance<Tile>();
        itemTile.flags = TileFlags.None;
        

        if (cell.Items[0].Sprite != null)
            itemTile.sprite = cell.Items[0].Sprite;
        else
            throw new NullReferenceException($"Item {cell.Items[0].DisplayName} has no sprite.");

        level.ItemTilemap.SetTile((Vector3Int)cell.Position, itemTile);
        level.ItemTilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
    }

    // Paint cells for targetting
    public static void PaintCells(Level level, List<Cell> cells)
    {
        Tile lineTargetOverlay = ScriptableObject.CreateInstance<Tile>();
        lineTargetOverlay.flags = TileFlags.None;
        lineTargetOverlay.sprite = Database.LineTargetOverlay;
        level.TargettingTilemap.ClearAllTiles();
        foreach (Cell cell in cells)
            level.TargettingTilemap.SetTile((Vector3Int)cell.Position, lineTargetOverlay);
    }

    // Unpaint cells for targetting
    public static void UnpaintCells(Level level)
        => level.TargettingTilemap.ClearAllTiles();
}

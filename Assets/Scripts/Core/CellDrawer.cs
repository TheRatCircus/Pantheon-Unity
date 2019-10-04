// CellDrawer.cs
// Jerome Martina

#define DEBUG_CELLDRAW
#undef DEBUG_CELLDRAW

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.Core;
using Pantheon.World;

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
            VisualiseDrawnCell(cell);
        }
    }

    [System.Diagnostics.Conditional("DEBUG_CELLDRAW")]
    public static void LogCellDrawing(int cellsDrawn)
    {
        Debug.Log($"Drawing {cellsDrawn} cells...");
    }

    [System.Diagnostics.Conditional("DEBUG_CELLDRAW")]
    public static void VisualiseDrawnCell(Cell cell)
    {
        Pantheon.Debug.Visualisation.MarkCell(cell, 5);
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

            if (cell.Splattered)
            {
                Color darkRed = Color.red;
                darkRed.r -= .2f;
                DrawSplatter(level, cell.Position, darkRed);
            }
                
            if (cell.Feature != null)
            {
                DrawFeature(level, cell);
            }
                
            if (cell.Revealed && cell.Items.Count > 0)
            {
                if (cell.Actor == null)
                {
                    DrawItem(level, cell);
                }
                else
                {
                    level.ItemTilemap.SetTile((Vector3Int)cell.Position, null);
                }
            }  
        }
    }

    public static void DrawTerrain(Level level, Cell cell)
    {
        level.TerrainTilemap.SetTile((Vector3Int)cell.Position,
            cell.TerrainData.RuleTile);
        level.TerrainTilemap.SetColor((Vector3Int)cell.Position,
            cell.Visible ? Color.white : Color.grey);
    }

    public static void DrawFeature(Level level, Cell cell)
    {
        if (cell.Feature.RuleTile != null)
        {
            level.FeatureTilemap.SetTile((Vector3Int)cell.Position,
                cell.Feature.RuleTile);
            level.FeatureTilemap.SetColor((Vector3Int)cell.Position,
                cell.Visible ? Color.white : Color.grey);
            return;
        }

        Tile featureTile = ScriptableObject.CreateInstance<Tile>();
        featureTile.flags = TileFlags.None;

        if (cell.Feature.Sprite != null)
            featureTile.sprite = cell.Feature.Sprite;
        else
            throw new NullReferenceException
                ($"Feature {cell.Feature.DisplayName} has no sprite.");

        level.FeatureTilemap.SetTile((Vector3Int)cell.Position, featureTile);
        level.FeatureTilemap.SetColor((Vector3Int)cell.Position,
            cell.Visible ? Color.white : Color.grey);
    }

    public static void DrawItem(Level level, Cell cell)
    {
        Tile itemTile = ScriptableObject.CreateInstance<Tile>();
        itemTile.flags = TileFlags.None;
        
        if (cell.Items[0].Sprite != null)
            itemTile.sprite = cell.Items[0].Sprite;
        else
            throw new NullReferenceException
                ($"Item {cell.Items[0].DisplayName} has no sprite.");

        level.ItemTilemap.SetTile((Vector3Int)cell.Position, itemTile);
        level.ItemTilemap.SetColor((Vector3Int)cell.Position,
            cell.Visible ? Color.white : Color.grey);
    }

    public static void DrawSplatter(Level level, Vector2Int position,
        Color color)
    {
        level.SplatterTilemap.SetTile((Vector3Int)position,
            Database.SplatterTile);
        Cell cell = level.GetCell(position);
        if (!cell.Visible)
        {
            color.r -= .5f;
            color.g -= .5f;
            color.b -= .5f;
        }
        level.SplatterTilemap.SetColor((Vector3Int)position, color);
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

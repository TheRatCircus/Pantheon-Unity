// Draws tilemaps based on cell data

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.Core;
using Pantheon.World;

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
        foreach (Cell cell in level.Map)
            DrawTile(level, cell);
    }

    // Draw a cell's tiles
    public static void DrawTile(Level level, Cell cell)
    {
        if (!cell.Revealed)
            level.terrainTilemap.SetTile((Vector3Int)cell.Position, level.unknownTile);
        else
        {
            DrawTerrain(level, cell);

            if (cell.Feature != null)
                DrawFeature(level, cell);

            if (cell.Revealed && cell.Items.Count > 0)
                if (cell._actor == null)
                    DrawItem(level, cell);
                else
                    level.itemTilemap.SetTile((Vector3Int)cell.Position, null);
        }
    }

    // Draw a cell's terrain tile
    public static void DrawTerrain(Level level, Cell cell)
    {
        level.terrainTilemap.SetTile((Vector3Int)cell.Position, cell.TerrainData._tile);
        level.terrainTilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
    }

    // Draw a cell's feature tile
    public static void DrawFeature(Level level, Cell cell)
    {
        Tile featureTile = ScriptableObject.CreateInstance<Tile>();
        featureTile.flags = TileFlags.None;

        if (cell.Feature.Sprite != null)
            featureTile.sprite = cell.Feature.Sprite;
        else
            throw new NullReferenceException($"Feature {cell.Feature.name} has no sprite.");

        level.featureTilemap.SetTile((Vector3Int)cell.Position, featureTile);
        level.featureTilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
    }

    // Draw a cell's item tile
    public static void DrawItem(Level level, Cell cell)
    {
        Tile itemTile = ScriptableObject.CreateInstance<Tile>();
        itemTile.flags = TileFlags.None;
        

        if (cell.Items[0]._sprite != null)
            itemTile.sprite = cell.Items[0]._sprite;
        else
            throw new NullReferenceException($"Item {cell.Items[0].DisplayName} has no sprite.");

        level.itemTilemap.SetTile((Vector3Int)cell.Position, itemTile);
        level.itemTilemap.SetColor((Vector3Int)cell.Position, cell.Visible ? Color.white : Color.grey);
    }

    // Paint cells for targetting
    public static void PaintCells(Level level, List<Cell> cells)
    {
        Tile lineTargetOverlay = ScriptableObject.CreateInstance<Tile>();
        lineTargetOverlay.flags = TileFlags.None;
        lineTargetOverlay.sprite = Database.LineTargetOverlay;
        level.targettingTilemap.ClearAllTiles();
        foreach (Cell cell in cells)
            level.targettingTilemap.SetTile((Vector3Int)cell.Position, lineTargetOverlay);
    }

    // Unpaint cells for targetting
    public static void UnpaintCells(Level level)
        => level.targettingTilemap.ClearAllTiles();
}

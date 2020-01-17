// Cell.cs
// Jerome Martina

using Pantheon.Content;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// Handle for a cell's absolute position in the world.
    /// </summary>
    [Serializable]
    public struct CellHandle
    {
        public readonly ICellArea area;
        public readonly byte x;
        public readonly byte y;

        public CellHandle(ICellArea area, byte x, byte y)
        {
            this.area = area;
            this.x = x;
            this.y = y;
        }
    }

    [Flags]
    public enum CellFlag
    {
        Visible = (1 << 0),
        Revealed = (1 << 1)
    }

    /// <summary>
    /// Helper class for interacting with the abstract "cells" of the world;
    /// use only locally.
    /// </summary>
    [Serializable]
    public sealed class Cell
    {
        // The offset of each tile from Unity's true grid coords
        public const float TileOffsetX = .5f;
        public const float TileOffsetY = .5f;

        public readonly CellHandle handle;

        public byte X => handle.x;
        public byte Y => handle.y;
        public Vector2Int Position => new Vector2Int(handle.x, handle.y);

        public bool Visible
        {
            get => handle.area.GetFlag(handle.x, handle.y).HasFlag(CellFlag.Visible);
            set
            {
                if (value)
                    handle.area.AddFlag(handle.x, handle.y, CellFlag.Visible);
                else
                    handle.area.RemoveFlag(handle.x, handle.y, CellFlag.Visible);
            }
        }
        public bool Revealed
        {
            get => handle.area.GetFlag(handle.x, handle.y).HasFlag(CellFlag.Revealed);
            set
            {
                if (value)
                    handle.area.AddFlag(handle.x, handle.y, CellFlag.Revealed);
                else
                    handle.area.RemoveFlag(handle.x, handle.y, CellFlag.Revealed);
            }
        }

        public TerrainDefinition Terrain
        {
            get => handle.area.GetTerrain(handle.x, handle.y);
            set => handle.area.SetTerrain(handle.x, handle.y, value);
        }

        public bool Opaque => Terrain != null && Terrain.Opaque;
        public bool Blocked
        {
            get
            {
                if (Actor == null) return Terrain != null && Terrain.Blocked;
                else return true;
            }
        }
        public bool HasWall => Terrain != null && Terrain.Blocked;

        public Entity Actor => handle.area.ActorAt(handle.x, handle.y);

        // METHODS

        public Cell(CellHandle handle)
        {
            this.handle = handle;
        }

        /// <summary>
        /// Whether a cell can be walked on at all (does not factor entities).
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>True if cell exists, has ground, and is not blocked.</returns>
        public static bool Walkable(Cell cell)
            => cell != null && cell.Terrain != null && !cell.Terrain.Blocked;

        public void SetVisibility(bool visible, int fallOff)
        {
            if (!visible)
            {
                Visible = false;
                return;
            }
            else
            {
                if (fallOff > 100)
                    Visible = false;
                else
                {
                    Visible = true;
                    Revealed = true;
                }
            }
        }
    }
}

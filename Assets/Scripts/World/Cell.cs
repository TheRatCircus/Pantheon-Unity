// Cell.cs
// Jerome Martina

using System;
using UnityEngine;

namespace Pantheon.World
{
    [Serializable]
    public sealed class Cell
    {
        // The offset of each tile from Unity's true grid coords
        public const float TileOffsetX = .5f;
        public const float TileOffsetY = .5f;

        public Vector2Int Position { get; set; }

        public bool Visible { get; set; } = false;
        public bool Revealed { get; set; } = false;

        public TerrainDefinition Terrain { get; set; }

        public bool Opaque => Terrain.Opaque;
        public bool Blocked => Terrain.Opaque;

        public Cell(Vector2Int position) => Position = position;

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

        public override string ToString() => $"Cell: {Position}";
    }
}

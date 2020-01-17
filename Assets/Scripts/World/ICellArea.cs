// ICellArea.cs
// Jerome Martina

using Pantheon.Content;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// Implemented by chunks and levels, used to get a cell's container.
    /// </summary>
    public interface ICellArea
    {
        byte[] Terrain { get; }
        CellFlag[] Flags { get; }
        HashSet<Entity> Actors { get; }
        HashSet<Entity> Items { get; }
        HashSet<Entity> Features { get; }
        HashSet<Entity> Clouds { get; }

        TerrainDefinition GetTerrain(int x, int y);
        TerrainDefinition GetTerrain(Vector2Int pos);
        void SetTerrain(int x, int y, TerrainDefinition terrain);
        CellFlag GetFlag(int x, int y);
        void AddFlag(int x, int y, CellFlag flag);
        void RemoveFlag(int x, int y, CellFlag flag);
        List<Entity> ItemsAt(int x, int y);
        Entity ActorAt(int x, int y);
        Entity ActorAt(Vector2Int pos);
    }
}

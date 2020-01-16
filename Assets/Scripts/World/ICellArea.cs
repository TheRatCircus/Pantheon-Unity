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
        byte[] TerrainMap { get; }
        HashSet<Entity> Entities { get; }
        Vector2Int Size { get; }

        int Index(int x, int y);
        TerrainDefinition GetTerrain(int x, int y);
        void SetTerrain(int x, int y, TerrainDefinition terrain);
        CellFlag GetFlag(int x, int y);
        void AddFlag(int x, int y, CellFlag flag);
        void RemoveFlag(int x, int y, CellFlag flag);
        List<Entity> ItemsAt(int x, int y);
        Entity ActorAt(int x, int y);
    }
}

// Chunk.cs
// Jerome Martina

using Pantheon.Content;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon.World
{
    public sealed class Chunk : ICellArea
    {
        public const int Width = 20;
        public const int Height = 20;

        public Vector2Int Position { get; private set; }
        public readonly int offsetX;
        public readonly int offsetY;

        public byte[] Terrain { get; } = new byte[Width * Height];
        public CellFlag[] Flags { get; } = new CellFlag[Width * Height];

        public HashSet<Entity> Actors { get; private set; } = new HashSet<Entity>();
        public HashSet<Entity> Items { get; private set; } = new HashSet<Entity>();
        public HashSet<Entity> Features { get; private set; }
        public HashSet<Entity> Clouds { get; private set; }

        public Chunk(Vector2Int position)
        {
            Position = position;
            offsetX = Position.x * Width;
            offsetY = Position.y * Height;
        }

        /// <summary>
        /// Map 2D coords to this chunk's 1D arrays.
        /// </summary>
        public int Index(int x, int y)
        {
            int localX = x - offsetX;
            int localY = y - offsetY;

            return (Width * localX) + localY;
        }

        /// <summary>
        /// Map 2D coords to this chunk's 1D arrays.
        /// </summary>
        public int Index(Vector2Int pos)
        {
            int localX = pos.x - offsetX;
            int localY = pos.y - offsetY;

            return (Width * localX) + localY;
        }

        public bool Contains(int x, int y)
        {
            if (x >= offsetX + Width || y >= offsetY + Height)
                return false;
            else if (x < offsetX || y < offsetY)
                return false;
            else
                return true;
        }

        public bool Contains(Vector2Int pos)
        {
            if (pos.x >= offsetX + Width || pos.y >= offsetY + Height)
                return false;
            else if (pos.x < offsetX || pos.y < offsetY)
                return false;
            else
                return true;
        }

        public TerrainDefinition GetTerrain(int x, int y)
        {
            return Assets.GetTerrain(Terrain[Index(x, y)]);
        }

        public TerrainDefinition GetTerrain(Vector2Int pos)
        {
            return Assets.GetTerrain(Terrain[Index(pos.x, pos.y)]);
        }

        public void SetTerrain(int x, int y, TerrainDefinition terrain)
        {
            Terrain[Index(x, y)] = Assets.GetTerrainIndex(terrain);
        }

        public CellFlag GetFlag(int x, int y)
        {
            return Flags[Index(x, y)];
        }

        public void AddFlag(int x, int y, CellFlag flag)
        {
            Flags[Index(x, y)] |= flag;
        }

        public void RemoveFlag(int x, int y, CellFlag flag)
        {
            Flags[Index(x, y)] &= ~flag;
        }

        public List<Entity> ItemsAt(int x, int y)
        {
            Profiler.BeginSample("Entity Search");
            List<Entity> ret = new List<Entity>();
            foreach (Entity e in Items)
                if (e.Position == new Vector2Int(x, y))
                    ret.Add(e);
           
            Profiler.EndSample();
            return ret;
        }

        public Entity ActorAt(int x, int y)
        {
            Profiler.BeginSample("Entity Search");
            foreach (Entity e in Actors)
                if (e.Position == new Vector2Int(x, y))
                    return e;
            
            Profiler.EndSample();
            return null;
        }

        public Entity ActorAt(Vector2Int pos)
        {
            Profiler.BeginSample("Entity Search");
            foreach (Entity e in Actors)
                if (e.Position == new Vector2Int(pos.x, pos.y))
                    return e;                
            
            Profiler.EndSample();
            return null;
        }

        public Entity FirstItemAt(int x, int y)
        {
            Profiler.BeginSample("Entity Search");
            foreach (Entity e in Items)
                if (e.Position == new Vector2Int(x, y))
                    return e;
            
            Profiler.EndSample();
            return null;
        }

        public Entity FirstItemAt(Vector2Int pos)
        {
            Profiler.BeginSample("Entity Search");
            foreach (Entity e in Items)
                if (e.Position == pos)
                    return e;
            
            Profiler.EndSample();
            return null;
        }

        public bool CellHasActor(Vector2Int pos)
        {
            foreach (Entity actor in Actors)
            {
                if (actor.Position == pos)
                    return true;
            }
            return false;
        }

        public override string ToString() => $"Chunk: {Position}";
    }
}

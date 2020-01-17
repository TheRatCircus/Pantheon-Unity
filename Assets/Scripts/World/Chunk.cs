// Chunk.cs
// Jerome Martina

using Pantheon.Content;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon.World
{
    public sealed class Chunk
    {
        public const int Width = 20;
        public const int Height = 20;

        public Vector2Int Position { get; private set; }
        public byte[] Terrain { get; } = new byte[Width * Height];
        public CellFlag[] Flags { get; } = new CellFlag[Width * Height];

        public HashSet<Entity> Actors { get; private set; }
        public HashSet<Entity> Items { get; private set; }
        public HashSet<Entity> Features { get; private set; }
        public HashSet<Entity> Clouds { get; private set; }

        public Chunk(Vector2Int position) => Position = position;

        /// <summary>
        /// Map 2D coords to this chunk's 1D arrays.
        /// </summary>
        private byte Index(int x, int y) => (byte)((Width * x) + y);

        /// <summary>
        /// Map 2D coords to this chunk's 1D arrays.
        /// </summary>
        private byte Index(Vector2Int pos) => (byte)((Width * pos.x) + pos.y);

        /// <summary>
        /// Get the distance of this chunk in cells to global (0,0).
        /// </summary>
        public Vector2Int Offset()
        {
            return new Vector2Int(Width * Position.x, Height * Position.y);
        }

        public bool Contains(int x, int y)
        {
            Vector2Int offset = Offset();
            if (x >= offset.x + Width || y >= offset.y + Height)
                return false;
            else if (x < offset.x || y < offset.y)
                return false;
            else
                return true;
        }

        public bool Contains(Vector2Int pos)
        {
            Vector2Int offset = Offset();
            if (pos.x >= offset.x + Width || pos.y >= offset.y + Height)
                return false;
            else if (pos.x < offset.x || pos.y < offset.y)
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
    }
}

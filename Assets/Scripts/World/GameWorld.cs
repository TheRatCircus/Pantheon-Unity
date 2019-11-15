// GameWorld.cs
// Jerome Martina

using Pantheon.Gen;
using UnityEngine;
using System.Collections.Generic;
using Pantheon.Core;

namespace Pantheon.World
{
    [System.Serializable]
    public sealed class GameWorld
    {
        public Dictionary<int, Layer> Layers { get; private set; }
            = new Dictionary<int, Layer>();
        private Dictionary<string, Level> levels
            = new Dictionary<string, Level>();

        public Level ActiveLevel { get; set; }

        // Request level from level generator
        public event System.Func<Vector3Int, Level> LevelRequestEvent;

        /// <summary>
        /// Build a fresh world for a new game.
        /// </summary>
        public GameWorld(LevelGenerator gen)
        {
            LevelRequestEvent += gen.GenerateLayerLevel;
            Layer surface = new Layer(0);
            AddLayer(surface);
            gen.GenerateWorldOrigin();
            Level origin = surface.RequestLevel(Vector2Int.zero);
        }

        public GameWorld(Save save)
        {
            Layers = save.World.Layers;
            levels = save.World.levels;
            ActiveLevel = save.World.ActiveLevel;
        }

        /// <summary>
        /// Add a layer to the world and register its level request event.
        /// </summary>
        /// <param name="layer"></param>
        private void AddLayer(Layer layer)
        {
            layer.LevelRequestEvent += RequestLevel;
            Layers.Add(layer.ZLevel, layer);
        }

        /// <summary>
        /// Request a newly-generated level from the level generation machine.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public Level RequestLevel(Vector3Int pos)
        {
            return LevelRequestEvent.Invoke(pos);

            throw new System.ArgumentException(
                $"Invalid position for level: {pos}");
        }
    }
}

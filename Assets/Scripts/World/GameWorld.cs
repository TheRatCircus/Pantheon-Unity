// GameWorld.cs
// Jerome Martina

using Pantheon.Gen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    [Serializable]
    public sealed class GameWorld
    {
        private LevelGenerator gen;

        public Dictionary<int, Layer> Layers { get; private set; }
            = new Dictionary<int, Layer>();
        public Dictionary<string, Level> Levels { get; private set; }
            = new Dictionary<string, Level>();

        public Level ActiveLevel { get; set; }

        public GameWorld(LevelGenerator gen) => this.gen = gen;

        public void NewLayer(int z)
        {
            Layer layer = new Layer(z);
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
            Level level = gen.GenerateLayerLevel(pos);
            Levels.Add(level.ID, level);
            return level;

            throw new ArgumentException(
                $"Invalid position for level: {pos}");
        }
    }
}

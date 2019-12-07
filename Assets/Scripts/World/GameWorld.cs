// GameWorld.cs
// Jerome Martina

using Pantheon.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    [Serializable]
    public sealed class GameWorld
    {
        private static GameController ctrl;

        public Dictionary<int, Layer> Layers { get; private set; }
            = new Dictionary<int, Layer>();
        public Dictionary<string, Level> Levels { get; private set; }
            = new Dictionary<string, Level>();

        public Level ActiveLevel { get; set; }

        public static void InjectController(GameController ctrl)
        {
            if (GameWorld.ctrl == null)
                GameWorld.ctrl = ctrl;
        }

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
            Level level = ctrl.Generator.GenerateLayerLevel(pos);
            Layers.TryGetValue(pos.z, out Layer layer);
            return level;

            throw new ArgumentException(
                $"Invalid position for level: {pos}");
        }
    }
}

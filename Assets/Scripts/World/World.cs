// World.cs
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
        private LevelGenerator gen;

        public Dictionary<int, Layer> Layers { get; private set; }
            = new Dictionary<int, Layer>();
        private Dictionary<string, Level> levels 
            = new Dictionary<string, Level>();

        public Level ActiveLevel { get; set; }

        /// <summary>
        /// Build a fresh world for a new game.
        /// </summary>
        public GameWorld(LevelGenerator gen)
        {
            this.gen = gen;

            Layer surface = new Layer(0);
            gen.RegisterLayer(surface);
            BuilderPlan plan = BuilderPlan.Load("Plan_Valley");
            Builder builder = new Builder("Valley of Beginnings",
                "valley_0_0_0", plan);
            gen.LayerLevelBuilders.Add(Vector3Int.zero, builder);
            Layers.Add(surface.ZLevel, surface);
            ActiveLevel = surface.RequestLevel(Vector2Int.zero);
            gen.LayerLevelBuilders.Remove(Vector3Int.zero);
        }

        public GameWorld(Save save)
        {
            Layers = save.World.Layers;
            ActiveLevel = save.World.ActiveLevel;
        }
    }
}

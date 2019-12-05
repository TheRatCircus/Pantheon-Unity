// GameWorld.cs
// Jerome Martina

using Pantheon.Gen;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pantheon.World
{
    public sealed class GameWorld : MonoBehaviour
    {
        [SerializeField] GameObject layerPrefab;

        [SerializeField] LevelGenerator gen;

        public Dictionary<int, Layer> Layers { get; private set; }
            = new Dictionary<int, Layer>();
        private Dictionary<string, Level> levels
            = new Dictionary<string, Level>();

        public Level ActiveLevel { get; set; }

        // Request level from level generator
        public event System.Func<Vector3Int, Level> LevelRequestEvent;

        public void NewLayer(int z)
        {
            Layer layer = Instantiate(layerPrefab, transform).GetComponent<Layer>();
            layer.LevelRequestEvent += RequestLevel;
            layer.ZLevel = z;
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
            Layers.TryGetValue(pos.z, out Layer layer);
            level.transform.SetParent(layer.transform);
            return level;

            throw new System.ArgumentException(
                $"Invalid position for level: {pos}");
        }
    }
}

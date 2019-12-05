// LevelGenerator.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Gen
{
    /// <summary>
    /// Holds all level builders and executes them upon request.
    /// </summary>
    public sealed class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject levelPrefab = default;

        private AssetLoader loader;

        public Dictionary<Vector3Int, Builder> LayerLevelBuilders
        { get; private set; } = new Dictionary<Vector3Int, Builder>();
        public Dictionary<string, Builder> IDLevelBuilders
        { get; private set; } = new Dictionary<string, Builder>();

        private void Start()
        {
            loader = GetComponent<AssetLoader>();
        }

        public void GenerateWorldOrigin()
        {
            //Builder json = loader.Load<TextAsset>("Plan_Valley");
            //JsonSerializerSettings settings = new JsonSerializerSettings
            //{
            //    TypeNameHandling = TypeNameHandling.Auto,
            //    SerializationBinder = Serialization._builderStepBinder,
            //    Formatting = Formatting.Indented
            //};
            BuilderPlan plan = Resources.Load<BuilderPlan>("Plan_Valley");

            Builder builder = new Builder("Valley of Beginnings",
                "valley_0_0_0", plan);
            LayerLevelBuilders.Add(Vector3Int.zero, builder);
        }

        public Level GenerateLayerLevel(Vector3Int pos)
        {
            if (!LayerLevelBuilders.TryGetValue(
                new Vector3Int(pos.x, pos.y, pos.z),
                out Builder builder))
            {
                throw new ArgumentException(
                    $"No level builder at {pos} on layer {pos.z}.");
            }
            else
            {
                GameObject levelObj = Instantiate(levelPrefab);
                Level level = levelObj.GetComponent<Level>();
                builder.Run(level);
                LayerLevelBuilders.Remove(pos);
                return level;
            }
        }
    }
}

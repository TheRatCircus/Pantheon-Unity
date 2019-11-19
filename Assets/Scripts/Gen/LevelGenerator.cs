// LevelGenerator.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Core;
using Pantheon.World;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Pantheon.Gen
{
    /// <summary>
    /// Holds all level builders and executes them upon request.
    /// </summary>
    [Serializable]
    public sealed class LevelGenerator
    {
        public event Func<GameController> GetControllerEvent;

        public Dictionary<Vector3Int, Builder> LayerLevelBuilders
        { get; private set; } = new Dictionary<Vector3Int, Builder>();
        public Dictionary<string, Builder> IDLevelBuilders
        { get; private set; } = new Dictionary<string, Builder>();

        public LevelGenerator(GameController ctrl)
            => GetControllerEvent += ctrl.Get;

        public LevelGenerator(Save save, GameController ctrl)
        {
            LayerLevelBuilders = save.Generator.LayerLevelBuilders;
            IDLevelBuilders = save.Generator.IDLevelBuilders;

            GetControllerEvent += ctrl.Get;
        }

        public void GenerateWorldOrigin()
        {
            GameController ctrl = GetControllerEvent.Invoke();

            TextAsset json = ctrl.Loader.Load<TextAsset>("Plan_Valley");
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Serialization._builderStepBinder,
                Formatting = Formatting.Indented
            };
            BuilderPlan plan = JsonConvert.DeserializeObject<BuilderPlan>(
                json.text, settings);

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
                GameController ctrl = GetControllerEvent.Invoke();
                Level level = builder.Run(ctrl.Loader, ctrl.EntityFactory);
                LayerLevelBuilders.Remove(pos);
                return level;
            }
        }

        [OnSerializing]
        private void OnSerializing() => GetControllerEvent = null;
    }
}

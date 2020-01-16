// AssetLoader.cs
// Jerome Martina

#define DEBUG_ASSETLOADER
#undef DEBUG_ASSETLOADER

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.Gen;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pantheon.Core
{
    public sealed class AssetLoader : IAssetLoader
    {
        private AssetBundle bundle;

        private JsonSerializerSettings jsonSettings;
        private JsonSerializerSettings speciesSettings;
        private JsonSerializerSettings genericSettings;
        private JsonSerializerSettings planSettings;

        public AssetLoader()
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            System.Diagnostics.Debug.Assert(bundle != null);
            ConstructSettings();
        }

        private void ConstructSettings()
        {
            jsonSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders.entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(this),
                    new TileConverter(),
                    new RuleTileConverter(this),
                    new SpeciesDefinitionConverter(this),
                    new BodyPartConverter(this),
                    new StatusConverter(this)
                }
            };

            speciesSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders.entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(this),
                    new RuleTileConverter(this),
                    new BodyPartConverter(this)
                }
            };

            genericSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders.entity,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new SpriteConverter(this),
                    new RuleTileConverter(this)
                }
            };

            planSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders.builder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new TerrainConverter(this)
                }
            };
        }

        public bool AssetExists(string name) => bundle.Contains(name);

        public T Load<T>(string name) where T : Object
        {
            UnityEngine.Profiling.Profiler.BeginSample("AssetLoader.Load()");
            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");
            LogLoad($"Attempting to load asset '{name}'...");

            T obj = bundle.LoadAsset<T>(name);

            UnityEngine.Profiling.Profiler.EndSample();
            return obj;
        }

        public EntityTemplate LoadTemplate(string name)
        {
            UnityEngine.Profiling.Profiler.BeginSample("AssetLoader.Load()");
            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");
            LogLoad($"Attempting to load asset '{name}'...");

            TextAsset text = bundle.LoadAsset<TextAsset>(name);
            UnityEngine.Profiling.Profiler.EndSample();

            return JsonConvert.DeserializeObject<EntityTemplate>(text.text, jsonSettings);
        }

        public BuilderPlan LoadPlan(string name)
        {
            UnityEngine.Profiling.Profiler.BeginSample("AssetLoader.Load()");
            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");
            LogLoad($"Attempting to load asset '{name}'...");

            TextAsset text = bundle.LoadAsset<TextAsset>(name);
            UnityEngine.Profiling.Profiler.EndSample();

            return JsonConvert.DeserializeObject<BuilderPlan>(text.text, planSettings);
        }

        public SpeciesDefinition LoadSpeciesDef(string name)
        {
            UnityEngine.Profiling.Profiler.BeginSample("AssetLoader.Load()");
            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");
            LogLoad($"Attempting to load asset '{name}'...");

            TextAsset text = bundle.LoadAsset<TextAsset>(name);
            UnityEngine.Profiling.Profiler.EndSample();

            return JsonConvert.DeserializeObject<SpeciesDefinition>(text.text, speciesSettings);
        }

        public BodyPart LoadBodyPart(string name)
        {
            UnityEngine.Profiling.Profiler.BeginSample("AssetLoader.Load()");
            if (!bundle.Contains(name))
                throw new ArgumentException(
                    $"{name} not found in bundle {bundle.name}.");
            LogLoad($"Attempting to load asset '{name}'...");

            TextAsset text = bundle.LoadAsset<TextAsset>(name);
            UnityEngine.Profiling.Profiler.EndSample();

            return JsonConvert.DeserializeObject<BodyPart>(
                text.text, genericSettings);
        }

        [System.Diagnostics.Conditional("DEBUG_ASSETLOADER")]
        public void LogLoad(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        // TODO: Temporary

        public TerrainDefinition GetTerrain(byte index)
        {
            switch (index)
            {
                default:
                case 0:
                    return null;
                case 1:
                    return Load<TerrainDefinition>("Terrain_Grass");
                case 2:
                    return Load<TerrainDefinition>("Terrain_StoneWall");
                case 3:
                    return Load<TerrainDefinition>("Terrain_StoneFloor");
                case 4:
                    return Load<TerrainDefinition>("Terrain_Water");
            }
        }

        public byte GetTerrainIndex(TerrainDefinition terrain)
        {
            if (terrain == null)
                return 0;

            switch (terrain.name)
            {
                default:
                    return 0;
                case "Terrain_Grass":
                    return 1;
                case "Terrain_StoneWall":
                    return 2;
                case "Terrain_StoneFloor":
                    return 3;
                case "Terrain_Water":
                    return 4;
            }
        }
    }
}

// LevelGenTester.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Gen;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using Pantheon.World;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon
{
    public sealed class LevelGenTester : MonoBehaviour
    {
        [SerializeField] private GameObject levelObj = default;
        [SerializeField] private Tilemap terrainTilemap = default;
        private Level level;

        [SerializeField] private TextAsset builderJsonFile = default;
        [SerializeField] private int sizeX = 200;
        [SerializeField] private int sizeY = 200;

        public void RunBuilder()
        {
            UnityEngine.Debug.ClearDeveloperConsole();
            Stopwatch totalTime = Stopwatch.StartNew();
            Assets.LoadAssets();

            if (builderJsonFile == null)
            {
                UnityEngine.Debug.LogError(
                    "No builder given for test, aborting.");
                return;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._builder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new TerrainConverter(),
                    new TileConverter(),
                    new RuleTileConverter()
                }
            };

            Builder builder = JsonConvert.DeserializeObject<Builder>(
                builderJsonFile.text, settings);

            level = new Level(sizeX, sizeY);
            foreach (BuilderStep step in builder.Steps)
            {
                Stopwatch stepTime = Stopwatch.StartNew();
                step.Run(level);
                stepTime.Stop();
                UnityEngine.Debug.Log(
                    $"Step {step} took {stepTime.ElapsedMilliseconds} ms.");
            }
            level.Initialize();
            level.AssignGameObject(levelObj.transform);

            Stopwatch drawTime = Stopwatch.StartNew();
            foreach (Vector2Int c in level.Map)
            {
                level.SetVisibility(c.x, c.y, true);
                level.DrawTile(c);
            }
            drawTime.Stop();
            UnityEngine.Debug.Log(
                $"Drawing took {drawTime.ElapsedMilliseconds} ms.");

            totalTime.Stop();
            UnityEngine.Debug.Log(
                $"Done. Total time elapsed: {totalTime.ElapsedMilliseconds} ms.");
        }

        public void Clear()
        {
            level = null;
            terrainTilemap.ClearAllTiles();
            Assets.UnloadAssets();
        }
    }
}

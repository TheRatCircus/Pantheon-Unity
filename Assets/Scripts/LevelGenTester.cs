// LevelGenTester.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Content;
using Pantheon.Core;
using Pantheon.Gen;
using Pantheon.Serialization.Json;
using Pantheon.Serialization.Json.Converters;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Pantheon
{
    public sealed class LevelGenTester : MonoBehaviour
    {
        [SerializeField] private GameObject levelObj = default;
        private Level level;

        [SerializeField] private TextAsset planJsonFile = default;
        [SerializeField] private int sizeX = 200;
        [SerializeField] private int sizeY = 200;

        public void RunPlan()
        {
            Stopwatch totalTime = Stopwatch.StartNew();
            AssetBundle.UnloadAllAssetBundles(true);

            if (planJsonFile == null)
            {
                UnityEngine.Debug.LogError("No plan given for test, aborting.");
                return;
            }

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders._builder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new TerrainConverter()
                }
            };

            BuilderPlan plan = JsonConvert.DeserializeObject<BuilderPlan>(
                planJsonFile.text, settings);

            level = new Level();
            InitializeMap(level, sizeX, sizeY);
            foreach (BuilderStep step in plan.Steps)
            {
                Stopwatch stepTime = Stopwatch.StartNew();
                step.Run(level);
                stepTime.Stop();
                UnityEngine.Debug.Log(
                    $"Step {step} took {stepTime.ElapsedMilliseconds} ms.");
            }
            level.Initialize();
            level.AssignGameObject(levelObj.transform);
            level.ClearTilemaps();

            Stopwatch drawTime = Stopwatch.StartNew();
            foreach (Cell c in level.Map)
            {
                c.SetVisibility(true, -1);
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
            level.ClearTilemaps();
            level = null;
            AssetBundle.UnloadAllAssetBundles(true);
        }

        private void InitializeMap(Level level, int sizeX, int sizeY)
        {
            level.Size = new Vector2Int(sizeX, sizeY);
            level.Map = new Cell[sizeX, sizeY];

            int x = 0;
            for (; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    level.Map[x, y] = new Cell(new Vector2Int(x, y));
        }
    }
}

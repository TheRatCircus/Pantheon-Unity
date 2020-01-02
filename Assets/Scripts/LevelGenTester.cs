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
        private AssetLoader loader;

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

            loader = new AssetLoader();
            Locator.Loader = loader;

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = Binders.builder,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>()
                {
                    new TerrainConverter(loader)
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

        private void PopulateNPCs(BuilderPlan plan, Level level)
        {
            int minSpawns = level.Map.Length / 100;
            int maxSpawns = level.Map.Length / 90;

            int numSpawns = UnityEngine.Random.Range(minSpawns, maxSpawns);

            for (int i = 0; i < numSpawns; i++)
            {
                string id = GenericRandomPick<string>.Pick(plan.Population);
                EntityTemplate template = Locator.Loader.LoadTemplate(id);

                Cell cell;
                int attempts = 0;
                do
                {
                    if (attempts > 100)
                        throw new Exception
                            ($"No valid NPC spawn position found after " +
                            $"{attempts} tries.");

                    cell = level.RandomCell(true);
                    attempts++;

                } while (!Cell.Walkable(cell));

                Spawn.SpawnActor(template, level, cell);
            }
        }

        private void PopulateItems(Level level)
        {
            int points = 100;
            while (points > 0)
            {
                Cell cell = level.RandomCell(true);
                Entity item;
                if (RandomUtils.OneChanceIn(3)) // Relic
                {
                    item = Relic.MakeRelic();
                    points -= 9; // Relics take a total of 10 points
                }
                else
                {
                    EntityTemplate basic = loader.LoadTemplate(Tables.basicItems.Random());
                    item = new Entity(basic);
                }

                item.Move(level, cell);
                points--;
            }
        }
    }
}

// LevelGenerator.cs
// Jerome Martina

#define DEBUG_LEVELGEN
#undef DEBUG_LEVELGEN

using Pantheon.Content;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Gen
{
    /// <summary>
    /// Holds all level builders and executes them upon request.
    /// </summary>
    [Serializable]
    public sealed class LevelGenerator
    {
        [NonSerialized]
        private AssetLoader loader;
        public AssetLoader Loader { get => loader; set => loader = value; }

        public Dictionary<Vector3Int, Builder> LayerLevelBuilders
        { get; private set; } = new Dictionary<Vector3Int, Builder>();
        public Dictionary<string, Builder> IDLevelBuilders
        { get; private set; } = new Dictionary<string, Builder>();

        public LevelGenerator(AssetLoader loader) => Loader = loader;

        public void PlaceBuilders()
        {
            BuilderPlan planSubterrane = Loader.LoadPlan("Plan_Subterrane");
            BuilderPlan planReform = Loader.LoadPlan("Plan_Reformatory");

            Builder builderSubterrane = new Builder("The Subterrane",
                "sub_0_0_-2", planSubterrane);
            Builder builderReform = new Builder("The Reformatory",
                "reform_0_0_-1", planReform);

            LayerLevelBuilders.Add(new Vector3Int(0, 0, -2), builderSubterrane);
            LayerLevelBuilders.Add(new Vector3Int(0, 0, -1), builderReform);
        }

        public Level GenerateLayerLevel(Vector3Int pos)
        {
            if (!LayerLevelBuilders.TryGetValue(
                new Vector3Int(pos.x, pos.y, pos.z),
                out Builder builder))
            {
                throw new ArgumentException(
                    $"No level builder at {pos}.");
            }
            else
            {
                Level level = new Level();
                level.DisplayName = builder.DisplayName;
                level.ID = builder.ID;
                InitializeMap(level, 200, 200);
                foreach (BuilderStep step in builder.Plan.Steps)
                    step.Run(level);
                if (builder.Plan.Population != null)
                    PopulateNPCs(builder.Plan, level);
                PopulateItems(level);
                level.Initialize();
                LayerLevelBuilders.Remove(pos);
                return level;
            }
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
            DebugLogGeneration($"Spawned {numSpawns} enemies in {level}.");
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
                    EntityTemplate basic = Loader.LoadTemplate(Tables.basicItems.Random());
                    item = new Entity(basic);
                }
                
                item.Move(level, cell);
                points--;
            }
        }

        [System.Diagnostics.Conditional("DEBUG_LEVELGEN")]
        private void DebugLogGeneration(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }
    }
}

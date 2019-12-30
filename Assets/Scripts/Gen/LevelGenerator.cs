// LevelGenerator.cs
// Jerome Martina

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

        public void GenerateWorldOrigin()
        {
            BuilderPlan plan = Loader.LoadPlan("Plan_Subterrane");

            Builder builder = new Builder("The Subterrane",
                "sub_0_0_0", plan);
            LayerLevelBuilders.Add(Vector3Int.zero, builder);
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
    }
}

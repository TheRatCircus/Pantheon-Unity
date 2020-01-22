// GameWorld.cs
// Jerome Martina

using Pantheon.Gen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    [Serializable]
    public sealed class GameWorld
    {
        public Dictionary<int, Layer> Layers { get; private set; }
            = new Dictionary<int, Layer>();
        public Dictionary<string, Level> Levels { get; private set; }
            = new Dictionary<string, Level>();

        public Dictionary<Vector3Int, Builder> LayerLevelBuilders
        { get; private set; } = new Dictionary<Vector3Int, Builder>();
        public Dictionary<string, Builder> IDLevelBuilders
        { get; private set; } = new Dictionary<string, Builder>();

        public Level ActiveLevel { get; set; }

        public GameWorld()
        {
            BuilderPlan planSubterrane = Assets.BuilderPlans["Plan_Subterrane"];
            BuilderPlan planReform = Assets.BuilderPlans["Plan_Reformatory"];
            BuilderPlan planFlood = Assets.BuilderPlans["Plan_FloodPlain"];

            Builder builderSubterrane = new Builder("The Subterrane",
                "sub_0_0_-2", planSubterrane);
            Builder builderReform = new Builder("The Reformatory",
                "reform_0_0_-1", planReform);
            Builder builderFlood = new Builder("The Floodplain",
                "floodplain_0_0_0", planFlood);

            LayerLevelBuilders.Add(new Vector3Int(0, 0, -2), builderSubterrane);
            LayerLevelBuilders.Add(new Vector3Int(0, 0, -1), builderReform);
            LayerLevelBuilders.Add(new Vector3Int(0, 0, 0), builderFlood);
        }

        public void NewLayer(int z)
        {
            Layer layer = new Layer(z);
            layer.LevelRequestEvent += RequestLevel;
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
            Level level = GenerateLayerLevel(pos);
            Levels.Add(level.ID, level);
            return level;

            throw new ArgumentException(
                $"Invalid position for level: {pos}");
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
                Level level = new Level
                {
                    DisplayName = builder.DisplayName,
                    ID = builder.ID
                };
                InitializeMap(level, 200, 200);
                foreach (BuilderStep step in builder.Plan.Steps)
                    step.Run(level);
                if (builder.Plan.Population != null)
                    NPC.PopulateNPCs(builder.Plan, level);
                Items.PopulateItems(level);
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
    }
}

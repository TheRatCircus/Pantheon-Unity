// GameWorld.cs
// Jerome Martina

using Pantheon.Gen;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.World
{
    [Serializable]
    public sealed class GameWorld
    {
        public Dictionary<int, Layer> Layers { get; private set; }
            = new Dictionary<int, Layer>();
        public Dictionary<string, Level> Levels { get; private set; }
            = new Dictionary<string, Level>();

        [NonSerialized] private WorldPlan plan;
        public WorldPlan Plan { get => plan; set => plan = value; }

        public List<Connection> Connections { get; private set; }
            = new List<Connection>();

        // TODO: Move to GameController
        public Level ActiveLevel { get; set; }

        public Layer NewLayer(int z)
        {
            Layer layer = new Layer(z);
            Layers.Add(layer.ZLevel, layer);
            layer.LevelRequestEvent += GenerateLayerLevel;
            return layer;
        }

        public Level RequestLevel(Connection connection)
        {
            foreach (Level lvl in Levels.Values)
                foreach (Connection conn in lvl.Connections)
                    if (conn.Partner == connection)
                        return lvl;

            Level level = GenerateIDLevel(connection.Key);
            Levels.Add(level.ID, level);
            return level;
        }

        public Level GenerateLayerLevel(Vector3Int pos)
        {
            Builder builder = null;

            // TODO: Optimize
            foreach (Builder b in Plan.Builders.Values)
                if (b.Position == pos)
                    builder = b;

            if (builder == null)
                throw new ArgumentException($"No builder at {pos}");

            Level level = new Level
            {
                DisplayName = builder.DisplayName,
                ID = builder.ID
            };
            InitializeMap(level, builder.Size.x, builder.Size.y);
            foreach (BuilderStep step in builder.Steps)
                step.Run(level);
            if (builder.Population != null)
                NPC.PopulateNPCs(builder, level);
            Items.PopulateItems(level);

            if (builder.ConnectionRules != null)
                foreach (ConnectionRule connRule in builder.ConnectionRules)
                {
                    Connection connection = new Connection()
                    {
                        Position = level.RandomCell(true).Position,
                        Key = connRule.Key,
                        Tile = connRule.Tile
                    };
                    level.Connections.Add(connection);
                    Connections.Add(connection);
                }

            // If another level is set to connect to this one, make it so
            foreach (Connection otherConn in Connections)
            {
                if (otherConn.Key != level.ID)
                    continue;

                Cell connBCell = level.RandomCell(true);
                Connection connB = new Connection()
                {
                    Position = connBCell.Position,
                    Partner = otherConn,
                    Tile = Assets.GetTile<Tile>("StoneStairs_Down")
                };
                otherConn.Partner = connB;
            }

            level.Initialize();
            return level;
        }

        public Level GenerateIDLevel(string id)
        {
            if (!Plan.Builders.TryGetValue(id, out Builder builder))
            {
                throw new ArgumentException(
                    $"No level builder with ID {id}.");
            }

            Level level = new Level
            {
                DisplayName = builder.DisplayName,
                ID = builder.LevelID
            };

            InitializeMap(level, builder.Size.x, builder.Size.y);
            foreach (BuilderStep step in builder.Steps)
                step.Run(level);
            if (builder.Population != null)
                NPC.PopulateNPCs(builder, level);
            Items.PopulateItems(level);

            if (builder.ConnectionRules != null)
                foreach (ConnectionRule connRule in builder.ConnectionRules)
                {
                    Connection connection = new Connection()
                    {
                        Position = level.RandomCell(true).Position,
                        Key = connRule.Key,
                        Tile = connRule.Tile
                    };
                    level.Connections.Add(connection);
                    Connections.Add(connection);
                }

            // If another level is set to connect to this one, make it so
            foreach (Connection otherConn in Connections)
            {
                if (otherConn.Key != level.ID)
                    continue;

                Cell connBCell = level.RandomCell(true);
                Connection connB = new Connection()
                {
                    Position = connBCell.Position,
                    Partner = otherConn,
                    Tile = Assets.GetTile<Tile>("StoneStairs_Down")
                };
                otherConn.Partner = connB;
            }

            level.Initialize();
            return level;
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

    public sealed class WorldPlan
    {
        public Dictionary<string, Builder> Builders
        { get; set; } = new Dictionary<string, Builder>();
    }
}

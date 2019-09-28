// LevelBuilder.cs
// Jerome Martina

using Pantheon.World;
using UnityEngine;
using static Pantheon.Utils.Helpers;

namespace Pantheon.WorldGen
{
    public abstract class LevelBuilder
    {
        protected Layer layer;
        protected Vector2Int layerPos;

        public LevelBuilder(Layer layer, Vector2Int layerPos)
        {
            this.layer = layer;
            this.layerPos = layerPos;
        }

        public abstract void Generate(Level level);
    }

    public abstract class ZoneLevelBuilder : LevelBuilder
    {
        public delegate void ZoneConnectDelegate(Level level,
            CardinalDirection direction);

        // TODO: theme
        protected Zone zone;
        protected CardinalDirection wing;
        public ZoneConnectDelegate ZoneConnect { get; set; }

        public ZoneLevelBuilder(Layer layer, Vector2Int layerPos, Zone zone,
            CardinalDirection wing)
            : base(layer, layerPos)
        {
            this.wing = wing;
            this.zone = zone;
        }
    }

    public sealed class TransitionBuilder : LevelBuilder
    {
        // TODO: themes

        CardinalDirection start;
        CardinalDirection end;

        public TransitionBuilder(Layer layer, Vector2Int layerPos,
            CardinalDirection start, CardinalDirection end)
            : base(layer, layerPos)
        {
            this.start = start;
            this.end = end;
        }

        public override void Generate(Level level)
        {
            level.RefName = "transition";
            level.DisplayName = "TRANSITION";
            level.Layer = layer;
            level.LayerPos = layerPos;

            Zones.ValleyBasics(level);
            Layout.Enclose(level, TerrainType.StoneWall);
            Connect.TransitionConnect(level, start, end);
        }
    }

    public sealed class ValleyBuilder : ZoneLevelBuilder
    {
        public const int ValleySize = 64;
        public const int ValleyEnemies = 10;

        public ValleyBuilder(Layer layer, Vector2Int layerPos, Zone zone,
            CardinalDirection wing)
            : base(layer, layerPos, zone, wing) { }

        public override void Generate(Level level)
        {
            level.RefName = $"valley_{layerPos.x}_{layerPos.y}";
            level.DisplayName = $"{wing.Adjective()} Valley at {layerPos}";
            level.gameObject.name = level.RefName;
            level.Layer = layer;
            level.LayerPos = layerPos;

            Zones.ValleyBasics(level);

            if (wing == CardinalDirection.Centre)
            {
                // Terrain generation
                LevelRect rect = new LevelRect(new Vector2Int(0, 0),
                    new Vector2Int(level.LevelSize.x, level.LevelSize.y));

                Layout.FillRect(level, rect, FeatureType.WoodFence);
                BinarySpacePartition.BSP(level, TerrainType.Grass, 12);
                Layout.Enclose(level, TerrainType.StoneWall);
                foreach (Cell c in level.Map)
                    if (Utils.RandomUtils.OneChanceIn(6))
                        c.SetFeature(null); // Chop fence a bit

                NPCs.SpawnNPCs(level, ValleyEnemies, NPCPops.ValleyCentre);

                UnityEngine.Debug.Log("Spawning the player...");
                Core.Game.instance.LoadLevel(level);
                level.SpawnPlayer();
            }
            else
                Zones.GenerateValley(level);

            Connect.ConnectZone(level, wing, null);
            if (wing != CardinalDirection.Centre)
                ZoneConnect?.Invoke(level, wing);

            Vector2Int zonePos = CardinalToV2I(wing);

            zone.Levels[zonePos.x + 1, zonePos.y + 1] = level;
            UnityEngine.Debug.Log($"Registering level {level.RefName}" +
                $" in dictionary...");
            Core.Game.instance.RegisterLevel(level);
            CellDrawer.DrawLevel(level);
        }

        public override string ToString()
            => $"ValleyBuilder {layerPos} {layer.ZLevel} {wing}";
    }

    public sealed class DomainBuilder : LevelBuilder
    {
        public DomainBuilder(Layer layer, Vector2Int layerPos)
            : base(layer, layerPos)
        {
        }

        public override void Generate(Level level)
        {
            throw new System.NotImplementedException();
        }
    }
}

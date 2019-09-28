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

    public sealed class ZoneLevelBuilder : LevelBuilder
    {
        public const int ValleySize = 64;
        public const int ValleyEnemies = 10;

        public delegate void ZoneConnectDelegate(Level level,
            CardinalDirection direction);

        private Zone zone;
        private Vector2Int zonePos;
        private CardinalDirection wing;
        private ZoneTheme theme;
        public ZoneConnectDelegate ZoneConnect { get; set; }

        public ZoneLevelBuilder(Layer layer, Vector2Int layerPos, Zone zone,
            Vector2Int zonePos, CardinalDirection wing, ZoneTheme theme)
            : base(layer, layerPos)
        {
            this.wing = wing;
            this.zone = zone;
            this.zonePos = zonePos;
            this.theme = theme;
        }

        public override void Generate(Level level)
        {
            level.RefName = $"{theme.ThemeRef}_{layerPos.x}_{layerPos.y}";
            level.DisplayName = $"{wing.Adjective()} {theme.DisplayName}";
            level.gameObject.name = level.RefName;
            level.Layer = layer;
            level.LayerPos = layerPos;

            if (wing == CardinalDirection.Centre)
                theme.CentreGenDelegate.Invoke(level);
            else
                theme.OuterGenDelegate.Invoke(level);

            if (layerPos == Vector2Int.zero)
            {
                UnityEngine.Debug.Log("Spawning the player...");
                Core.Game.instance.LoadLevel(level);
                level.SpawnPlayer();
            }

            Connect.ConnectZone(level, wing, null);
            if (wing != CardinalDirection.Centre)
                ZoneConnect?.Invoke(level, wing);

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

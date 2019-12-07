// LevelGenerator.cs
// Jerome Martina

using Pantheon.Core;
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
        private static GameController ctrl;

        public Dictionary<Vector3Int, Builder> LayerLevelBuilders
        { get; private set; } = new Dictionary<Vector3Int, Builder>();
        public Dictionary<string, Builder> IDLevelBuilders
        { get; private set; } = new Dictionary<string, Builder>();

        public static void InjectController(GameController ctrl)
        {
            if (LevelGenerator.ctrl == null)
                LevelGenerator.ctrl = ctrl;
        }

        public void GenerateWorldOrigin()
        {
            BuilderPlan plan = ctrl.Loader.Load<BuilderPlan>("Plan_Valley");

            Builder builder = new Builder("Valley of Beginnings",
                "valley_0_0_0", plan);
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
                builder.Run(level);
                LayerLevelBuilders.Remove(pos);
                return level;
            }
        }
    }
}

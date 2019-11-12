// LevelGenerator.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.Gen
{
    public sealed class LevelGenerator
    {
        private GameObject levelPrefab;
        private GameObject worldObj;
        private EntityFactory factory;

        public Dictionary<Vector3Int, Builder> LayerLevelBuilders
        { get; private set; } = new Dictionary<Vector3Int, Builder>();
        public Dictionary<string, Builder> IDLevelBuilders
        { get; private set; } = new Dictionary<string, Builder>();

        public LevelGenerator(EntityFactory factory, GameObject levelPrefab,
            GameObject worldObj)
        {
            this.factory = factory;
            this.worldObj = worldObj;
            this.levelPrefab = levelPrefab;
        }

        public void RegisterLayer(Layer layer)
        {
            layer.LevelRequestEvent += GenerateLayerLevel;
        }

        public void GenerateLayerLevel(Layer layer, Vector2Int pos)
        {
            if (!LayerLevelBuilders.TryGetValue(
                new Vector3Int(pos.x, pos.y, layer.ZLevel),
                out Builder builder))
                throw new System.ArgumentException(
                    $"No level builder at {pos} on layer {layer.ZLevel}.");
            else
            {
                Level level = builder.Run(factory);
                GameObject levelObj = Object.Instantiate(
                    levelPrefab, worldObj.transform);
                level.SetLevelObject(levelObj);
                layer.Levels.Add(pos, level);
            }
        }
    }
}

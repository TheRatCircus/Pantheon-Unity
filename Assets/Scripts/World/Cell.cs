// Cell.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.ECS.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    public sealed class Cell
    {
        public Vector2Int Position { get; private set; }
        private List<Entity> entities = new List<Entity>();
        public Entity Blocker { get; private set; }
        public Entity Terrain { get; private set; }
        public bool Blocked => Blocker != null;

        public bool Visible { get; set; }
        public bool Revealed { get; set; }

        public Cell(Vector2Int position) => Position = position;

        public void AddEntity(Entity entity)
        {
            if (entities.Contains(entity))
                throw new ArgumentException(
                    "Attempt to add duplicate entity.");

            if (entity.HasComponent<Blocking>())
            {
                if (Blocker == null)
                    Blocker = entity;
                else
                    throw new Exception(
                        "Cell can only contain one blocking entity.");
            }

            if (entity.HasComponent<Ground>())
            {

                if (Terrain == null)
                    Terrain = entity;
                else
                    throw new Exception(
                        "Cell can only contain one terrain entity.");
            }

            entities.Add(entity);
        }

        public bool RemoveEntity(Entity entity)
        {
            if (entity.HasComponent<Blocking>())
            {
                if (Blocker == null)
                    throw new Exception("A blocker was not cached.");
                else
                    Blocker = null;
            }

            if (entity.HasComponent<Ground>())
            {
                if (Terrain == null)
                    throw new Exception("Terrain was not cached.");
                else
                    Terrain = null;
            }

            return entities.Remove(entity);
        }

        public override string ToString() => $"cell at {Position}";
    }
}

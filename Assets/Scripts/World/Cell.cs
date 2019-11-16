// Cell.cs
// Jerome Martina

using Pantheon.ECS;
using Pantheon.ECS.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    [Serializable]
    public sealed class Cell
    {
        // The offset of each tile from Unity's true grid coords
        public const float TileOffsetX = .5f;
        public const float TileOffsetY = .5f;

        public Vector2Int Position { get; set; }
        public List<Entity> Entities { get; } = new List<Entity>();
        public Entity Blocker { get; private set; }
        public Entity Terrain { get; private set; } // What tile gets drawn?
        public bool Blocked => Blocker != null;
        public bool Walled
        {
            get
            {
                if (Terrain == null)
                    return false;
                else
                    return Terrain.Archetype == EntityArchetype.Wall;
            }
        }

        public bool Visible { get; set; }
        public bool Revealed { get; set; }

        public Cell(Vector2Int position) => Position = position;

        public void AddEntity(Entity entity)
        {
            if (Entities.Contains(entity))
                throw new ArgumentException(
                    "Attempt to add duplicate entity.");

            // Walls take priority
            if (entity.Archetype == EntityArchetype.Wall)
                Terrain = entity;
            else if (entity.Archetype == EntityArchetype.Ground)
                if (Terrain == null)
                    Terrain = entity;

            if (entity.HasComponent<Blocking>())
            {
                if (Blocker == null)
                    Blocker = entity;
                else
                    throw new Exception(
                        "Cell can only contain one blocking entity.");
            }

            Entities.Add(entity);
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

            if (entity == Terrain)
                Terrain = null;

            return Entities.Remove(entity);
        }

        public void SetVisibility(bool visible, int fallOff)
        {
            if (!visible)
            {
                Visible = false;
                return;
            }
            else
            {
                if (fallOff > 100)
                    Visible = false;
                else
                {
                    Visible = true;
                    Revealed = true;
                }
            }
        }

        public override string ToString() => $"cell at {Position}";
    }
}

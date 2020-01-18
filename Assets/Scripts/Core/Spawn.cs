// Spawn.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Content;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Core
{
    /// <summary> Functions for spawning entities. </summary>
    public static class Spawn
    {
        /// <summary>
        /// Spawn an actor, add it to the turn scheduler.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="level"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static Entity SpawnActor(EntityTemplate template, Level level, Vector2Int cell)
        {
            Entity entity = new Entity(template);
            Locator.Scheduler.AddActor(entity.GetComponent<Actor>());
            entity.Move(level, cell);
            if (entity.Level.CellIsVisible(cell))
                Locator.Player.VisibleActors.Add(entity);
            return entity;
        }
    }
}

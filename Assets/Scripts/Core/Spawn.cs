// Spawn.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.World;

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
        public static Entity SpawnActor(EntityTemplate template, Level level, Cell cell)
        {
            Entity entity = new Entity(template);
            Locator.Scheduler.AddActor(entity.GetComponent<Actor>());
            entity.Move(level, cell);
            return entity;
        }
    }
}

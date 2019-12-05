// Spawn.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Core
{
    /// <summary>
    /// Functions for spawning GameObject-based entities.
    /// </summary>
    public static class Spawn
    {
        private static TurnScheduler scheduler;

        public static void Init(TurnScheduler scheduler)
        {
            Spawn.scheduler = scheduler;
        }

        /// <summary>
        /// Instantiate an actor at a cell based on a prefab.
        /// </summary>
        /// <returns>The Actor script component of the new actor.</returns>
        public static Actor SpawnActor(GameObject npcPrefab, Level level, Cell cell)
        {
            GameObject npcObj = Object.Instantiate(
                npcPrefab,
                cell.Position.ToVector3(),
                new Quaternion(),
                level.transform);
            Actor actor = npcObj.GetComponent<Actor>();
            actor.gameObject.name = actor.Name;
            actor.Level = level;
            scheduler.AddActor(actor);
            actor.Move(level, cell);
            return actor;
        }
    }
}
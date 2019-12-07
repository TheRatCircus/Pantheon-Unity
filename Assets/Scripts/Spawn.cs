// Spawn.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Core
{
    /// <summary> Functions for spawning entities. </summary>
    public static class Spawn
    {
        private static GameObject gameObjPrefab;
        private static TurnScheduler scheduler;

        public static void Init(TurnScheduler scheduler, GameObject gameObjPrefab)
        {
            Spawn.gameObjPrefab = gameObjPrefab;
            Spawn.scheduler = scheduler;
        }

        /// <summary>
        /// Spawn an actor, add it to the turn scheduler, and give it a GameObject.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="level"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static Entity SpawnActor(EntityTemplate template, Level level, Cell cell)
        {
            Entity entity = new Entity(template);
            scheduler.AddActor(entity.GetComponent<Actor>());
            entity.Move(level, cell);
            //entity.GameObjects[0] = AssignGameObject(entity);
            return entity;
        }

        public static void AssignGameObject(Entity entity)
        {
            GameObject entityObj = Object.Instantiate(
                gameObjPrefab,
                entity.Cell.Position.ToVector3(),
                new Quaternion(),
                entity.Level.EntitiesTransform);

            entityObj.name = entity.Name;
            EntityWrapper wrapper = entityObj.GetComponent<EntityWrapper>();
            wrapper.Entity = entity;
            SpriteRenderer sr = entityObj.GetComponent<SpriteRenderer>();
            sr.sprite = entity.Sprite;

            entity.GameObjects = new GameObject[1];
            entity.GameObjects[0] = entityObj;
        }
    }
}

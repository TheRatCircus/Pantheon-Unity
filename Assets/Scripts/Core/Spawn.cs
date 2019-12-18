// Spawn.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Content;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Core
{
    /// <summary> Functions for spawning entities. </summary>
    public static class Spawn
    {
        private static GameController ctrl;

        public static void InjectController(GameController ctrl)
        {
            if (Spawn.ctrl == null)
                Spawn.ctrl = ctrl;
        }

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
            SchedulerLocator.Service.AddActor(entity.GetComponent<Actor>());
            entity.Move(level, cell);
            if (entity.Cell.Visible)
                ctrl.PlayerControl.VisibleActors.Add(entity);
            return entity;
        }

        public static void AssignGameObject(Entity entity)
        {
            GameObject entityObj = Object.Instantiate(
                ctrl.GameObjectPrefab,
                entity.Cell.Position.ToVector3(),
                new Quaternion(),
                entity.Level.EntitiesTransform);

            entityObj.name = entity.Name;
            EntityWrapper wrapper = entityObj.GetComponent<EntityWrapper>();
            wrapper.Entity = entity;
            SpriteRenderer sr = entityObj.GetComponent<SpriteRenderer>();
            sr.sprite = entity.Flyweight.Sprite;

            if (!entity.Cell.Visible)
                sr.enabled = false; 

            entity.GameObjects = new GameObject[1];
            entity.GameObjects[0] = entityObj;
        }
    }
}

// EntityManager.cs
// Jerome Martina

using Pantheon.ECS.Components;
using System;
using System.Collections.Generic;

namespace Pantheon.ECS
{
    /// <summary>
    /// Holds all entities and components in the running game.
    /// </summary>
    [Serializable]
    public sealed class EntityManager
    {
        private int id = 0;

        public Dictionary<int, Entity> Entities { get; private set; }
            = new Dictionary<int, Entity>();
        public Entity Player { get; private set; }

        public HashSet<Entity> ActiveEntities { get; private set; }
            = new HashSet<Entity>();

        public List<Actor> ActorComponents { get; private set; }
            = new List<Actor>(); // List so it can be used like a queue

        public Entity GetEntity(int guid)
        {
            if (!Entities.TryGetValue(guid, out Entity ret))
                throw new ArgumentException(
                    $"Entity of GUID {guid} not found.");
            else
                return ret;
        }

        public void AddEntity(Entity entity)
        {
            entity.SetGUID(id++);

            if (entity.TryGetComponent(out Actor a))
                ActorComponents.Add(a);

            if (entity.HasComponent<Player>())
                Player = entity;

            Entities.Add(entity.GUID, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity.GUID);
        }
    }
}

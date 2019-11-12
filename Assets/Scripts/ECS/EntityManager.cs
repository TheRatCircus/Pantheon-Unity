// EntityManager.cs
// Jerome Martina

using Pantheon.ECS.Components;
using System.Collections.Generic;

namespace Pantheon.ECS
{
    /// <summary>
    /// Holds all entities and components in the running game.
    /// </summary>
    public sealed class EntityManager
    {
        public Dictionary<int, Entity> Entities { get; private set; }
            = new Dictionary<int, Entity>();
        public Entity Player { get; private set; }

        public HashSet<Health> HealthComponents { get; private set; }
            = new HashSet<Health>();
        public HashSet<Position> PositionComponents { get; private set; }
            = new HashSet<Position>();
        public List<Actor> ActorComponents { get; private set; }
            = new List<Actor>(); // List so we can use it like a queue
        public Entity GetEntity(int guid)
        {
            if (!Entities.TryGetValue(guid, out Entity ret))
                throw new System.ArgumentException(
                    $"Entity of GUID {guid} not found.");
            else
                return ret;
        }

        public void AddEntity(Entity entity)
        {
            entity.SetGUID(Entities.Count);

            if (entity.TryGetComponent(out Health h))
                HealthComponents.Add(h);
            if (entity.TryGetComponent(out Position p))
                PositionComponents.Add(p);
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

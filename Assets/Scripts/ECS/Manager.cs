// Manager.cs
// Jerome Martina

using Pantheon.ECS.Components;
using System.Collections.Generic;

namespace Pantheon.ECS
{
    /// <summary>
    /// Holds all entities and components in the running game.
    /// </summary>
    public sealed class Manager
    {
        public Dictionary<int, Entity> Entities { get; private set; }
            = new Dictionary<int, Entity>();

        public HashSet<Health> HealthComponents { get; private set; }
            = new HashSet<Health>();
        public HashSet<Position> PositionComponents { get; private set; }
            = new HashSet<Position>();

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
            Entities.Add(Entities.Count - 1, entity);
        }
    }
}

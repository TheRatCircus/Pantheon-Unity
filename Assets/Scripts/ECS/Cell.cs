// Cell.cs
// Jerome Martina

using System.Collections.Generic;

namespace Pantheon.ECS
{
    public sealed class Cell
    {
        private List<Entity> entities = new List<Entity>();

        public void AddEntity(Entity entity)
        {
            if (entities.Contains(entity))
                throw new System.ArgumentException(
                    "Attempt to add duplicate entity.");

            entities.Add(entity);
        }
    }
}

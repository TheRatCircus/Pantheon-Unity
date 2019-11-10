// Position.cs
// Jerome Martina

using Pantheon.Utils;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Position : BaseComponent
    {
        public Level Level { get; set; }
        public Cell Cell { get; set; }

        public Level DestinationLevel { get; set; }
        public Cell DestinationCell { get; set; }

        public Position(Level level, Cell cell)
        {
            Level = level;
            Cell = cell;
        }

        public void Move(Entity entity, Level level, Cell destination)
        {
            if (Cell != null)
                Cell.RemoveEntity(entity);

            if (entity.TryGetComponent(out UnityGameObject go))
                go.GameObject.transform.position = Helpers.V2IToV3(
                    Cell.Position);

            destination.AddEntity(entity);

            Level = level;
            Cell = destination;
        }
    }
}

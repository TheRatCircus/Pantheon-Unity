// PositionSystem.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.ECS.Components;
using System;

namespace Pantheon.ECS.Systems
{
    public sealed class PositionSystem : ComponentSystem
    {
        public PositionSystem(EntityManager mgr) : base(mgr) { }

        public override void UpdateComponents()
        {
            foreach (Position pos in mgr.PositionComponents)
                UpdatePosition(pos);
        }

        public void UpdatePosition(Position pos)
        {
            if (pos.DestinationCell != null)
            {
                Entity e = mgr.GetEntity(pos.GUID);

                if (pos.Cell != null)
                    pos.Cell.RemoveEntity(e);

                pos.Cell = pos.DestinationCell;
                pos.DestinationCell = null;
                pos.Cell.AddEntity(e);

                // Level change always comes with a cell change, 
                // so check for a destination level here
                if (pos.DestinationLevel != null)
                {
                    // Verify that destination cell
                    // belongs to destination level
                    if (!pos.DestinationLevel.Map.ContainsKey(pos.Cell.Position))
                        throw new Exception(
                            $"{pos.DestinationLevel} does not contain {pos.Cell}.");
                    //if (p.DestinationLevel.Map[p.Cell.Position.x, p.Cell.Position.y] == null)
                    //    throw new Exception();

                    pos.Level = pos.DestinationLevel;
                    pos.DestinationLevel = null;
                }

                if (e.TryGetComponent(out UnityGameObject go)
                    && go.GameObject != null)
                    go.GameObject.transform.position = Helpers.V2IToV3(
                        pos.Cell.Position);
            }
        }
    }
}
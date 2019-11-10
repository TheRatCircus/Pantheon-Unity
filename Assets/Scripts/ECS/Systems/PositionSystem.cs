// PositionSystem.cs
// Jerome Martina

using Pantheon.ECS.Systems;
using Pantheon.ECS.Components;
using System;

namespace Pantheon.ECS.Systems
{
    public sealed class PositionSystem : ComponentSystem
    {
        public override void UpdateComponents()
        {
            foreach (Position p in mgr.PositionComponents)
            {
                if (p.DestinationCell != null)
                {
                    if (p.Cell != null)
                        p.Cell.RemoveEntity(entity);

                    p.Cell = p.DestinationCell;
                    p.DestinationCell = null;

                    // Level change always comes with a cell change, 
                    // so check for a destination level here
                    if (p.DestinationLevel != null)
                    {
                        // Verify that destination cell 
                        // belongs to destination level
                        if (!p.DestinationLevel.Map.ContainsKey(p.Cell.Position))
                            throw new Exception();

                        p.Level = p.DestinationLevel;
                        p.DestinationLevel = null;
                    }
                }

                

                if (entity.TryGetComponent(out UnityGameObject go))
                    go.GameObject.transform.position = Helpers.V2IToV3(
                        Cell.Position);

                destination.AddEntity(entity);
            }
        }
    }
}
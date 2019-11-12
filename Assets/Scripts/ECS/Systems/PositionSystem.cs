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
            foreach (Position p in mgr.PositionComponents)
            {
                if (p.DestinationCell != null)
                {
                    Entity e = mgr.GetEntity(p.GUID);

                    if (p.Cell != null)
                        p.Cell.RemoveEntity(e);

                    p.Cell = p.DestinationCell;
                    p.DestinationCell = null;
                    p.Cell.AddEntity(e);

                    // Level change always comes with a cell change, 
                    // so check for a destination level here
                    if (p.DestinationLevel != null)
                    {
                        // Verify that destination cell
                        // belongs to destination level
                        if (!p.DestinationLevel.Map.ContainsKey(p.Cell.Position))
                            throw new Exception(
                                $"{p.DestinationLevel} does not contain {p.Cell}.");
                        //if (p.DestinationLevel.Map[p.Cell.Position.x, p.Cell.Position.y] == null)
                        //    throw new Exception();

                        p.Level = p.DestinationLevel;
                        p.DestinationLevel = null;
                    }

                    if (e.TryGetComponent(out UnityGameObject go))
                        go.GameObject.transform.position = Helpers.V2IToV3(
                            p.Cell.Position);
                }
            }
        }
    }
}
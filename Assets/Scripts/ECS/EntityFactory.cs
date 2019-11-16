// EntityFactory.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS.Components;
using Pantheon.ECS.Systems;
using Pantheon.ECS.Templates;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.ECS
{
    public sealed class EntityFactory
    {
        private PositionSystem posSystem;

        private event System.Func<GameController> GetControllerEvent;

        public EntityFactory(GameController ctrl, PositionSystem posSystem)
        {
            GetControllerEvent += ctrl.Get;
            this.posSystem = posSystem;
        }

        private Entity NewEntity(Template template, bool flyweightOnly)
        {
            Entity e = new Entity(template, flyweightOnly);

            return e;
        }

        public Entity NewEntityAt(Template template, Level level, Cell cell)
        {
            GameController ctrl = GetControllerEvent.Invoke();
            EntityManager mgr = ctrl.Manager;
            Entity e = NewEntity(template, false);

            if (ctrl.World != null &&
                level == ctrl.World.ActiveLevel) // Never true during level generation
            {
                // New entity needs visualization
                if (e.TryGetComponent(out UnityGameObject go))
                {
                    GameObject obj = Object.Instantiate(ctrl.GameObjectPrefab);
                    go.GameObject = obj;
                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    sr.sprite = template.Sprite;
                    obj.name = template.EntityName;
                    // GameObject position gets updated by PositionSystem
                }
            }

            if (!e.TryGetComponent(out Position pos))
            {
                Position newPos = new Position(level, cell);
                e.AddComponent(newPos);
                mgr.AddEntity(e);
                // Pos update must always follow manager add
                posSystem.UpdatePosition(newPos);
            }
            else
            {
                pos.SetDestination(level, cell);
                mgr.AddEntity(e);
                posSystem.UpdatePosition(pos);
            }

            return e;
        }

        public Entity FlyweightEntityAt(Template template, Level level,
            Cell cell)
        {
            GameController ctrl = GetControllerEvent.Invoke();
            EntityManager mgr = ctrl.Manager;
            Entity e = NewEntity(template, true);

            // Entity has no position, so bypass position system
            cell.AddEntity(e);
            return e;
        }
    }
}

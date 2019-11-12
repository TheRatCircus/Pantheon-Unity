// EntityFactory.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS.Templates;
using Pantheon.World;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon.ECS
{
    public sealed class EntityFactory
    {
        private GameObject gameObjectPrefab;

        private EntityManager mgr;

        public EntityFactory(EntityManager mgr)
        {
            this.mgr = mgr;

            AssetLoader loader = new AssetLoader();
            gameObjectPrefab = loader.Load<GameObject>("GameObjectPrefab");
            loader.Unload(false);
        }

        private Entity NewEntity(Template template)
        {
            Entity e = new Entity(template.Name, template);

            if (e.TryGetComponent(out UnityGameObject go))
            {
                GameObject obj = Object.Instantiate(gameObjectPrefab);
                go.GameObject = obj;
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.sprite = e.GetComponent<GameObjectSprite>().Sprite;
                obj.name = template.Name;
            }
            
            return e;
        }

        public Entity NewEntityAt(Template template, Level level,
            Cell cell)
        {
            Entity e = NewEntity(template);
            if (!e.TryGetComponent(out Position pos))
                e.AddComponent(new Position(level, cell));
            else
                pos.SetDestination(level, cell);
            mgr.AddEntity(e);
            return e;
        }
    }
}

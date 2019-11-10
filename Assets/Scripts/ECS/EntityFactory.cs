// EntityFactory.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS.Templates;
using UnityEngine;

namespace Pantheon.ECS
{
    public sealed class EntityFactory
    {
        public static Entity NewEntity(Template template)
        {
            Entity e = new Entity(template.Name, template);

            if (e.TryGetComponent(out UnityGameObject go))
            {
                GameObject genericNPCPrefab = Assets.Load<GameObject>(
                    "GameObjectPrefab");
                GameObject obj = Object.Instantiate(genericNPCPrefab);
                go.GameObject = obj;
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.sprite = e.GetComponent<GameObjectSprite>().Sprite;
                obj.name = template.Name;
            }

            return e;
        }

        public static Entity NewEntityAt(Template template, Level level,
            Cell cell)
        {
            Entity e = NewEntity(template);
            if (!e.HasComponent<Position>())
                e.AddComponent(new Position(level, cell));
            return e;
        }
    }
}

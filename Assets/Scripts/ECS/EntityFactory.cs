// EntityFactory.cs
// Jerome Martina

using Pantheon.ECS.Components;
using Pantheon.ECS.Templates;
using UnityEngine;

namespace Pantheon.ECS
{
    public static class EntityFactory
    {
        public static Entity NewEntity(Template template)
        {
            Entity e = new Entity(template);
            if (e.HasComponent<UnityGameObject>())
            {
                GameObject genericNPCPrefab = Assets.Load<GameObject>("GenericNPC");
                GameObject obj = Object.Instantiate(genericNPCPrefab);
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.sprite = e.GetComponent<GameObjectSprite>().Sprite;
            }
            return e;
        }
    }
}

// GameObjectSprite.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class GameObjectSprite : BaseComponent
    {
        [SerializeField] private Sprite sprite = default;
        public Sprite Sprite => sprite;

        public GameObjectSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public override BaseComponent Clone()
        {
            return new GameObjectSprite(sprite);
        }
    }
}

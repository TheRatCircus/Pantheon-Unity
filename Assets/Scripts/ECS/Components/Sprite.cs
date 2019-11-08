// Sprite.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class GameObjectSprite : BaseComponent
    {
        [SerializeField] private Sprite sprite = default;
        public Sprite Sprite => sprite;
    }
}

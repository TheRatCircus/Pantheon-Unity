// UnityTile.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class UnityTile : BaseComponent
    {
        [SerializeField] private RuleTile tile = default;
        public RuleTile Tile { get => tile; set => tile = value; }

        public UnityTile(RuleTile tile)
        {
            this.tile = tile;
        }
    }
}

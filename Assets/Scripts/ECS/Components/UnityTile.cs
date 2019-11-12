// UnityTile.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class UnityTile : BaseComponent
    {
        [SerializeField] private RuleTile tile = default;
        public RuleTile Tile {
            get
            {
                if (tile == null)
                    throw new System.Exception();
                else
                    return tile;
            }
        }

        public UnityTile(RuleTile tile)
        {
            this.tile = tile;
        }
    }
}

// WorldMap.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.Tilemaps;
using Pantheon.Core;
using Pantheon.World;

namespace Pantheon.UI
{
    public sealed class WorldMap : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap = null;

        [SerializeField] private Tile mapTile = null;

        private Vector2Int currentPosition;

        // Start is called before the first frame update
        void Start()
        {
            tilemap.SetTile((Vector3Int)Game.GetPlayer().level.LayerPos,
                mapTile);
            Game.instance.OnLevelChangeEvent += MapLevel;
        }

        private void MapLevel(Level level)
        {
            tilemap.SetColor((Vector3Int)currentPosition, Color.grey);
            if (!level.Visited)
                tilemap.SetTile((Vector3Int)level.LayerPos, mapTile);
            currentPosition = level.LayerPos;
            tilemap.SetColor((Vector3Int)currentPosition, Color.white);
        }
    }
}

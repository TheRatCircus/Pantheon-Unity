// Landmark.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// A prefab of objects placed in the world at generation time.
    /// </summary>
    [CreateAssetMenu(fileName = "New Landmark",
        menuName = "Pantheon/Content/Landmark")]
    public sealed class Landmark : ScriptableObject
    {
        [SerializeField] private GameObject prefab = null;
        [SerializeField] private LandmarkRef reference = LandmarkRef.None;
        private Tilemap tilemap = null;
        [SerializeField] private List<TerrainType> terrain
            = new List<TerrainType>();

        public LandmarkRef Reference => reference;
        public Vector2Int Size => (Vector2Int)tilemap.size;

        public void Initialize()
        {
            tilemap = prefab.GetComponent<Tilemap>();
            tilemap.CompressBounds();
        }

        public TerrainType GetTerrain(int x, int y)
        {
            TileBase marker = tilemap.GetTile
                ((Vector3Int)new Vector2Int(x, y));

            if (marker == null)
                return TerrainType.None;

            int markerIndex = int.Parse(marker.name.Split('_')[1]);
            return terrain[markerIndex];
        }

        /// <summary>
        /// Place a landmark at a position.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="position"></param>
        public static bool Build(LandmarkRef reference,
            World.Level level, Vector2Int position)
        {
            Landmark landmark = Core.Database.GetLandmark(reference);
            landmark.Initialize();

            if (!level.Contains(position + landmark.Size))
            {
                //Debug.Visualisation.MarkPos(position, Color.red, 120);
                UnityEngine.Debug.LogWarning
                    ("Position too close to level bounds.");
                return false;
            }
                //throw new System.ArgumentException
                //    ("Position too close to level bounds.");

            for (int x = position.x; x < landmark.Size.x + position.x; x++)
                for (int y = position.y; y < landmark.Size.y + position.y; y++)
                {
                    //Debug.Visualisation.MarkPos(new Vector2Int(x, y),
                    //    Color.green, 120);
                    level.Map[x, y].SetTerrain(landmark.GetTerrain(
                        x - position.x, y - position.y));
                }
            return true;
        }
    }

    public enum LandmarkRef
    {
        None,
        Keep
    }
}
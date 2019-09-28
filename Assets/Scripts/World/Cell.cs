// Cell.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// The fundamental component of levels, containing terrain, items, and up
    /// to one actor.
    /// </summary>
    [System.Serializable]
    public sealed class Cell
    {
        // Statics
        // The offset of each tile from Unity's true grid coords
        public const float TileOffsetX = .5f;
        public const float TileOffsetY = .5f;

        [SerializeField] [ReadOnly] private Vector2Int position;

        [SerializeField] [ReadOnly] private TerrainData terrainData;
        // Can cell be moved through?
        [SerializeField] [ReadOnly] private bool blocked = true;
        // Can cell be seen through?
        [SerializeField] [ReadOnly] private bool opaque = true;
        [SerializeField] [ReadOnly] private Connection connection;

        // Status
        // Is this cell within view?
        [SerializeField] private bool visible = false;
        // Is this cell known?
        [SerializeField] private bool revealed = false;

        // Contents of cell
        [SerializeField] [ReadOnly] private Actor actor = null;
        [SerializeField] [ReadOnly] List<Item> items = new List<Item>();
        public Feature Feature { get; private set; } = null;
        public Altar Altar { get; set; }

        #region Properties

        public Vector2Int Position { get => position; set => position = value; }
        public bool Blocked
        {
            get
            {
                if (Feature != null)
                    return (blocked || Feature.Blocked);
                else
                    return blocked;
            }
        }
        public bool Opaque
        {
            get
            {
                if (Feature != null)
                    return (opaque || Feature.Opaque);
                else
                    return opaque;
            }
            set => opaque = value;
        }
        public bool Visible => visible;
        public bool Revealed { get => revealed; set => revealed = value; }
        public Actor Actor { get => actor; set => actor = value; }
        public List<Item> Items => items;
        public TerrainData TerrainData { get => terrainData; }
        public Connection Connection { get => connection; set => connection = value; }

        #endregion

        public Cell(Vector2Int position) => this.position = position;

        public void SetVisibility(bool visible, int fallOff)
        {
            if (!visible)
            {
                this.visible = false;
                return;
            }
            else
            {
                if (fallOff > 100)
                    this.visible = false;
                else
                {
                    this.visible = true;
                    revealed = true;
                }
            }
        }

        public void Reveal() => revealed = true;

        public void SetTerrain(TerrainType type)
        {
            if (Feature != null)
                SetFeature(FeatureType.None);

            if (type == TerrainType.None)
                return;

            TerrainData terrainData = Database.GetTerrain(type);
            this.terrainData = terrainData;
            opaque = terrainData.Opaque;
            blocked = terrainData.Blocked;
        }

        /// <summary>
        /// Set this cell's feature type and adjust its attributes accordingly.
        /// </summary>
        /// <param name="featureData">This cell's new Feature
        /// (can be FeatureType.None).</param>
        public void SetFeature(FeatureType feature)
        {
            if (feature == FeatureType.None)
            {
                Feature = null;
                opaque = terrainData.Opaque;
                blocked = terrainData.Blocked;
                return;
            }

            FeatureData featureData = Database.GetFeature(feature);

            Feature = new Feature(featureData);
            opaque = featureData.Opaque;
            blocked = featureData.Blocked;
        }

        public void SetAltar(Altar altar)
        {
            Altar = altar;
            FeatureData featureData = Database.GetFeature(altar.FeatureType);
            Feature = new Feature(featureData);
            Feature.DisplayName
                = $"{featureData.DisplayName} to {altar.Idol.DisplayName}";
        }

        public override string ToString()
        {
            string ret;

            if (revealed)
                ret = "Unknown terrain";
            else
            {
                if (actor != null)
                    ret = actor.ActorName;
                else if (Feature != null)
                    ret = Feature.DisplayName;
                else if (Items.Count > 0)
                    ret = Items[0].DisplayName;
                else
                    ret = terrainData.DisplayName;
            }

            return ret;
        }
    }
}

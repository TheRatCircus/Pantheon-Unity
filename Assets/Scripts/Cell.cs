// Cell.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Actors;

namespace Pantheon.World
{
    /// <summary>
    /// The fundamental component of levels, containing terrain, items, and up
    /// to one actor.
    /// </summary>
    [System.Serializable]
    public class Cell
    {
        // Statics
        // The offset of each tile from Unity's true grid coords
        public const float TileOffsetX = .5f;
        public const float TileOffsetY = .5f;

        [SerializeField] [ReadOnly] private Vector2Int position;

        [SerializeField] [ReadOnly] private TerrainData terrainData;
        [SerializeField] [ReadOnly] private bool blocked = true; // Can cell be moved through?
        [SerializeField] [ReadOnly] private bool opaque = true; // Can cell be seen through?
        [SerializeField] [ReadOnly] private Connection connection;

        // Status
        [SerializeField] private bool visible = false; // Is this cell within view?
        [SerializeField] private bool revealed = false; // Is this cell known?

        // Contents of cell
        [SerializeField] [ReadOnly] private Actor actor = null;
        [SerializeField] [ReadOnly] List<Item> items = new List<Item>();

        #region Properties

        public Vector2Int Position { get => position; set => position = value; }
        public bool Blocked { get => blocked; set => blocked = value; }
        public bool Opaque
        {
            get
            {
                if (Feature != null)
                    return (opaque && Feature.Opaque);
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
        public Feature Feature { get; private set; } = null;
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

        /// <summary>
        /// Set this cell's terrain type and adjust its attributes accordingly.
        /// </summary>
        /// <param name="terrainData">The TerrainData which should define this
        /// cell's new terrain.</param>
        public void SetTerrain(TerrainData terrainData)
        {
            if (Feature != null)
                SetFeature(null);

            this.terrainData = terrainData;
            opaque = terrainData.Opaque;
            blocked = terrainData.Blocked;
        }

        /// <summary>
        /// Set this cell's feature type and adjust its attributes accordingly.
        /// </summary>
        /// <param name="feature">This cell's new Feature.</param>
        public void SetFeature(Feature feature)
        {
            if (feature == null)
            {
                Feature = null;
                opaque = false;
                blocked = false;
                return;
            }

            Feature = feature;
            opaque = feature.Opaque;
            blocked = feature.Blocked;
        }

        // Check if this cell can be walked into
        public bool IsWalkableTerrain()
        {
            if (Feature != null)
                return (!blocked && !Feature.Blocked);
            else
                return !blocked;
        }

        // toString override: returns name of tile, position, contained actor
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
                    ret = Feature.name;
                else if (Items.Count > 0)
                    ret = Items[0].DisplayName;
                else
                    ret = terrainData.DisplayName;
            }

            return ret;
        }
    }
}

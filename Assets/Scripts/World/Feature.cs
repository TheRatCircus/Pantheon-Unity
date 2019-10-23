// Feature.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// An object which can co-exist in a cell with terrain, e.g. a tree.
    /// </summary>
    public sealed class Feature
    {
        public string DisplayName { get; set; }
        public bool Opaque { get; set; }
        public bool Blocked { get; set; }
        public Sprite Sprite { get; set; }
        public RuleTile RuleTile { get; set; }

        public Feature(FeatureDef featureDef)
        {
            DisplayName = featureDef.DisplayName;
            Opaque = featureDef.Opaque;
            Blocked = featureDef.Blocked;
            Sprite = featureDef.Sprite;
            RuleTile = featureDef.RuleTile;
        }
    }
}
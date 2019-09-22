// Feature.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.World
{
    public class Feature
    {
        public string DisplayName { get; set; }
        public bool Opaque { get; set; }
        public bool Blocked { get; set; }
        public Sprite Sprite { get; set; }
        public RuleTile RuleTile { get; set; }

        public Feature(FeatureData featureData)
        {
            DisplayName = featureData.DisplayName;
            Opaque = featureData.Opaque;
            Blocked = featureData.Blocked;
            Sprite = featureData.Sprite;
            RuleTile = featureData.RuleTile;
        }
    }
}
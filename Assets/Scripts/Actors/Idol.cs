// Idol.cs
// Jerome Martina

using Pantheon.Core;

namespace Pantheon.Actors
{
    /// <summary>
    /// Represents abstract information about an Idol.
    /// </summary>
    public class Idol
    {
        public string DisplayName { get; set; }
        public string RefName { get; set; }
        public Gender Gender { get; set; }

        public Aspect Aspect { get; set; }

        public bool HasAnAltar { get; set; } = false;
        public Faction Religion { get; set; }

        public override string ToString() => $"{RefName} ({Aspect})";
    }

    public class Altar
    {
        public readonly Idol Idol;
        public readonly FeatureType FeatureType;

        public Altar(Idol idol, FeatureType featureType)
        {
            Idol = idol;
            FeatureType = featureType;
        }
    }
}

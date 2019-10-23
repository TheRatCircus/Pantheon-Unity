// Idol.cs
// Jerome Martina

using Pantheon.Core;

namespace Pantheon
{
    /// <summary>
    /// Represents abstract information about an Idol.
    /// </summary>
    [System.Serializable]
    public sealed class Idol
    {
        public string DisplayName { get; set; }
        public string RefName { get; set; }
        public Gender Gender { get; set; }

        public Aspect Aspect { get; set; }

        public bool HasAnAltar { get; set; } = false;
        public Faction Religion
        {
            get
            {
                Game.instance.Religions.TryGetValue(this,
                    out Faction religion);
                return religion;
            }
        }
        

        public override string ToString() => $"{RefName} ({Aspect})";
    }

    public sealed class Altar
    {
        public readonly Idol Idol;
        public readonly FeatureID FeatureType;

        public Altar(Idol idol, FeatureID featureType)
        {
            Idol = idol;
            FeatureType = featureType;
        }
    }
}

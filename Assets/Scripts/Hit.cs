// Hit.cs
// Jerome Martina

namespace Pantheon
{
    public struct Hit
    {
        public readonly Damage[] damages;
    }

    public sealed class Damage
    {
        public DamageType Type { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}

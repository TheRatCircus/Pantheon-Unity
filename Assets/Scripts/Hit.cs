// Hit.cs
// Jerome Martina

namespace Pantheon
{
    public struct Hit
    {
        public readonly HitDamage[] damages;

        public Hit(Damage[] damages)
        {
            this.damages = new HitDamage[damages.Length];
            for (int i = 0; i < this.damages.Length; i++)
                this.damages[i] = new HitDamage(damages[i]);
        }
    }
}

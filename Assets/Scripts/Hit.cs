// Hit.cs
// Jerome Martina

using Pantheon.Components;

namespace Pantheon
{
    /// <summary>
    /// Data passed to an actor's TakeHit() when it receives an incoming hit.
    /// </summary>
    public struct Hit
    {
        public readonly int Damage;
        public readonly DamageType DamageType;

        // Constructor
        public Hit(int minDamage, int maxDamage, DamageType type)
        {
            if (minDamage > maxDamage)
                throw new System.Exception
                    ("A Hit has a higher min than max damage.");

            Damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
            DamageType = type;
        }
    }
}
